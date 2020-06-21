using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using LiteDB;
using LiteDB.Engine;
using ServiceInterfaces;
using ServiceInterfaces.Dto;

namespace LiteDbSsRepositoryService
{
    [SuppressMessage("Microsoft.Design", "CA1307")] // LiteDb requires field.Equals 
    public class LiteDbSsRepository : ISsRepository, IDisposable
    {
        public LiteDbSsRepository(string filename)
        {
            OpenDb = () => new LiteDatabase(filename, SetupBsonMapper());
        }

        public LiteDbSsRepository()
        {
            tempStream = new TempStream();
            OpenDb = () => new LiteDatabase(tempStream, SetupBsonMapper());
        }

        public List<SsResource> GetResources()
        {
            var usages = LockCollection.Query().ToList().ToDictionary(ru => ru.ResourceShortName);
            var resources = ResourceCollection.Query().ToList();
            var users = GetUsers().ToDictionary(u => u.UserName);
            foreach (var resource in resources)
            {
                var usage = usages.GetValueOrDefault(resource.ShortName, null);
                if (usage == null)
                    continue;
                resource.Locking = new LockingInfo
                {
                    LockedBy = users[usage.UserName],
                    LockedAt = usage.LockedAt,
                    Comment = usage.Comment,
                };
            }
            return resources;
        }

        public IEnumerable<SsUser> GetUsers()
        {
            return UserCollection
                .Query()
                .ToList()
                .Select(user => new SsUser(user.UserName, "*", user.Role));
        }

        public SsResource GetResource(string shortName)
        {
            var resource = ResourceCollection
                .Query()
                .Where(res => res.ShortName == SsResource.NormalizeShortName(shortName))
                .SingleOrDefault();
            if (resource == null)
                return null;

            shortName = resource.ShortName;
            var usage = LockCollection
                .Query()
                .Where(ru => ru.ResourceShortName.Equals(shortName))
                .SingleOrDefault();
            if (usage == null)
                return resource;

            resource.Locking = new LockingInfo
            {
                Comment = usage.Comment,
                LockedAt = usage.LockedAt,
                LockedBy = GetUser(usage.UserName).AsUser()
            };
            return resource;
        }

        public void SaveResource(SsResource ssResource)
        {
            ResourceCollection.Upsert(ssResource);
        }

        public SsUserSecret GetUser(string userName)
        {
            return UserCollection
                .Query()
                .Where(user => user.UserName.Equals(SsUserSecret.NormalizeUserName(userName)))
                .SingleOrDefault();
        }

        public void SaveUser(SsUserSecret user)
        {
            UserCollection.Upsert(user);
        }

        public LockingInfo GetLocking(SsResource resource)
        {
            var usage = LockCollection
                .Query()
                .Where(ru => ru.ResourceShortName.Equals(resource.ShortName))
                .SingleOrDefault();

            if (usage == null)
                return null;

            return new LockingInfo
            {
                LockedBy = GetUser(usage.UserName).AsUser(),
                LockedAt = usage.LockedAt,
                Comment = usage.Comment
            };
        }

        public void Lock(SsResource resource, string userName, string comment)
        {
            Contract.Requires(resource != null);
            LockCollection.Insert(new ResourceUsage
            {
                ResourceShortName = resource.ShortName,
                UserName = userName,
                Comment = comment,
                LockedAt = TimeService.Instance.Now
            });
        }

        public void Release(SsResource resource)
        {
            LockCollection.DeleteMany(ru => ru.ResourceShortName == resource.ShortName);
        }

        public bool DeleteResource(string shortName)
        {
            var resource = GetResource(shortName);
            if (resource == null)
                return false;
            ResourceCollection.DeleteMany(r => r.ShortName.Equals(resource.ShortName));
            return true;
        }

        public bool DeleteUser(string userName)
        {
            var user = GetUser(userName);
            if (user == null)
                return false;
            userName = user.UserName; // normalized
            UserCollection.DeleteMany(r => r.UserName.Equals(userName));
            LockCollection.DeleteMany(ru => ru.UserName.Equals(userName));
            return true;
        }

        public bool DbImport(string content)
        {
            var impEx = JsonSerializer.Deserialize(content);
            foreach (var collectionName in new[] {
                ResourceCollectionName,
                UserCollectionName,
                LockCollectionName,
            })
            {
                var collection = Db.GetCollection(collectionName);
                collection.DeleteAll();
                var resources = ((BsonArray)impEx[collectionName]).Cast<BsonDocument>();
                collection.InsertBulk(resources);
            }
            return true;
        }

        public string DbExport()
        {
            var impEx = new BsonDocument();
            foreach (var collectionName in new[] {
                ResourceCollectionName,
                UserCollectionName,
                LockCollectionName,
            })
            {
                impEx[collectionName] = new BsonArray(
                    Db.GetCollection(collectionName).Query().ToEnumerable());
            }
            return JsonSerializer.Serialize(impEx);
        }

        public T Transaction<T>(Func<T> action)
        {
            Contract.Requires(action != null);
            lock (transactionLock)
            {
                if (Db != null)
                {
                    return action();
                }
                else
                {
#pragma warning disable IDE0063
                    // using statement can't be simplified
                    using (var db = OpenDb())
#pragma warning restore
                    {
                        Db = db;
                        try
                        {
                            return action();
                        }
                        finally
                        {
                            Db = null;
                        }
                    }
                }
            }
        }

        #region IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    tempStream?.Close();
                    Db?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable

        #region Private fields
        private readonly Func<LiteDatabase> OpenDb;
        private readonly TempStream tempStream;
        private LiteDatabase Db;
        private bool disposedValue;
        private static readonly object transactionLock = new object();

        private static BsonMapper SetupBsonMapper()
        {
            var mapper = new BsonMapper();
            mapper.Entity<SsResource>()
                .Id(r => r.ShortName)
                .Ignore(r => r.Locking);
            mapper.Entity<SsUser>()
                .Id(u => u.UserName);
            return mapper;
        }

        private const string ResourceCollectionName = "resources";
        private const string UserCollectionName = "users";
        private const string LockCollectionName = "locks";
        private ILiteCollection<SsResource> ResourceCollection => Db.GetCollection<SsResource>(ResourceCollectionName);
        private ILiteCollection<SsUserSecret> UserCollection => Db.GetCollection<SsUserSecret>(UserCollectionName);
        private ILiteCollection<ResourceUsage> LockCollection => Db.GetCollection<ResourceUsage>(LockCollectionName);
        #endregion Private fields
    }
}
