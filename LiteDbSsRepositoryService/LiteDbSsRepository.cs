using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using LiteDB;
using ServiceInterfaces;
using ServiceInterfaces.Dto;

namespace LiteDbSsRepositoryService
{
    public class LiteDbSsRepository : ISsRepository
    {
        public string Filename { get; }
        public LiteDbSsRepository(string filename)
        {
            Filename = filename;
        }
        public List<SsResource> GetAll()
        {
            return Db.GetCollection<SsResource>("resources").Query().ToList();
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

        private LiteDatabase Db = null;
        private static object transactionLock = new object();

        public void Transaction(Action action)
        {
            Contract.Requires(action != null);
            lock (transactionLock)
            {
                if (Db != null)
                {
                    action();
                }
                else
                {
                    using (var db = new LiteDatabase(Filename))
                    {
                        Db = db;
                        try
                        {
                            action();
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
    }

    public class ResourceUsage
    {
        public string ResourceShortName { get; set; }
        public string UserName { get; set; }
    }
}
