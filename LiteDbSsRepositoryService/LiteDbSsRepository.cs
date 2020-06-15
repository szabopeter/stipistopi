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
    public class LiteDbSsRepository : ISsRepository, IDisposable
    {
        private readonly Func<LiteDatabase> OpenDb;
        private readonly TempStream tempStream;
        private LiteDatabase Db;
        private bool disposedValue;
        private static readonly object transactionLock = new object();

        public LiteDbSsRepository(string filename)
        {
            OpenDb = () => new LiteDatabase(filename);
        }

        public LiteDbSsRepository()
        {
            tempStream = new TempStream();
            OpenDb = () => new LiteDatabase(tempStream);
        }

        public List<SsResource> GetResources()
        {
            return Db.GetCollection<SsResource>("resources").Query().ToList();
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
            return Db.GetCollection<SsResource>("resources")
                .Query()
                .Where(res => res.ShortName == SsResource.NormalizeShortName(shortName))
                .SingleOrDefault();
        }

        public void SaveResource(SsResource ssResource)
        {
            Db.GetCollection<SsResource>("resources").Upsert(ssResource);
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

        [SuppressMessage("Microsoft.Design", "CA1307")] // LiteDb requires field.Equals 
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

        [SuppressMessage("Microsoft.Design", "CA1307")] // LiteDb requires field.Equals 
        public SsUserSecret GetLockingUser(SsResource resource)
        {
            var usage = Db.GetCollection<ResourceUsage>()
                .Query()
                .Where(ru => ru.ResourceShortName.Equals(resource.ShortName))
                .SingleOrDefault();
            if (usage == null)
                return null;
            return GetUser(usage.UserName);
        }

        public void SetLockingUser(SsResource resource, SsUserSecret user)
        {
            Contract.Requires(resource != null);
            if (user != null)
            {
                Db.GetCollection<ResourceUsage>().Insert(new ResourceUsage
                {
                    ResourceShortName = resource.ShortName,
                    UserName = user.UserName
                });
            }
            else
            {
                Db.GetCollection<ResourceUsage>().DeleteMany(ru => ru.ResourceShortName == resource.ShortName);
            }
        }

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
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    public class ResourceUsage
    {
        public string ResourceShortName { get; set; }
        public string UserName { get; set; }
    }
}
