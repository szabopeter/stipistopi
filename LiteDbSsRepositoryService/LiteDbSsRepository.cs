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
            OpenDb = () => new LiteDatabase(filename);
            SetupBsonMapping();
        }

        public LiteDbSsRepository()
        {
            tempStream = new TempStream();
            OpenDb = () => new LiteDatabase(tempStream);
            SetupBsonMapping();
        }

        private static bool IsBsonMapperConfigured = false;
        private static object BsonConfigurationLock = new object();
        private static void SetupBsonMapping()
        {
            lock (BsonConfigurationLock)
            {
                if (IsBsonMapperConfigured)
                    return;

                var mapper = BsonMapper.Global;
                mapper.Entity<SsResource>()
                    .Id(r => r.ShortName)
                    .Ignore(r => r.Locking);
                IsBsonMapperConfigured = true;
            }
        }

        public List<SsResource> GetResources()
        {
            var usages = Db.GetCollection<ResourceUsage>().Query().ToList().ToDictionary(ru => ru.ResourceShortName);
            var resources = Db.GetCollection<SsResource>("resources").Query().ToList();
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
            return Db.GetCollection<SsUserSecret>("users")
                .Query()
                .ToList()
                .Select(user => new SsUser(user.UserName, "*", user.Role));
        }

        public SsResource GetResource(string shortName)
        {
            var resource = Db.GetCollection<SsResource>("resources")
                .Query()
                .Where(res => res.ShortName == SsResource.NormalizeShortName(shortName))
                .SingleOrDefault();
            if (resource == null)
                return null;

            shortName = resource.ShortName;
            var usage = Db.GetCollection<ResourceUsage>()
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
            Db.GetCollection<SsResource>("resources").Upsert(ssResource);
        }

        public SsUserSecret GetUser(string userName)
        {
            return Db.GetCollection<SsUserSecret>("users")
                .Query()
                .Where(user => user.UserName.Equals(SsUserSecret.NormalizeUserName(userName)))
                .SingleOrDefault();
        }

        public void SaveUser(SsUserSecret user)
        {
            Db.GetCollection<SsUserSecret>("users").Upsert(user);
        }

        public LockingInfo GetLocking(SsResource resource)
        {
            var usage = Db.GetCollection<ResourceUsage>()
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
            Db.GetCollection<ResourceUsage>().Insert(new ResourceUsage
            {
                ResourceShortName = resource.ShortName,
                UserName = userName,
                Comment = comment,
                LockedAt = TimeService.Instance.Now
            });
        }

        public void Release(SsResource resource)
        {
            Db.GetCollection<ResourceUsage>().DeleteMany(ru => ru.ResourceShortName == resource.ShortName);
        }

        public bool DeleteResource(string shortName)
        {
            var resource = GetResource(shortName);
            if (resource == null)
                return false;
            Db.GetCollection<SsResource>("resources").DeleteMany(r => r.ShortName.Equals(resource.ShortName));
            return true;
        }

        public bool DeleteUser(string userName)
        {
            var user = GetUser(userName);
            if (user == null)
                return false;
            userName = user.UserName; // normalized
            Db.GetCollection<SsUserSecret>("users").DeleteMany(r => r.UserName.Equals(userName));
            Db.GetCollection<ResourceUsage>().DeleteMany(ru => ru.UserName.Equals(userName));
            return true;
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
        #endregion Private fields
    }
}
