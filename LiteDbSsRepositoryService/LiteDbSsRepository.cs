using System;
using System.Collections.Generic;
using ServiceInterfaces;
using ServiceInterfaces.Dto;

namespace LiteDbSsRepositoryService
{
    public class LiteDbSsRepository : ISsRepository
    {
        public List<SsResource> GetAll()
        {
            throw new NotImplementedException();
        }

        public SsUser GetLockedBy(SsResource ssr)
        {
            throw new NotImplementedException();
        }

        public SsResource GetResource(string shortName)
        {
            throw new NotImplementedException();
        }

        public void SaveResource(SsResource ssResource)
        {
            throw new NotImplementedException();
        }

        public void Transaction(Action action)
        {
            throw new NotImplementedException();
        }

        public SsUserSecret GetUser(string userName)
        {
            throw new NotImplementedException();
        }

        public void SaveUser(SsUserSecret user)
        {
            throw new NotImplementedException();
        }

        public SsUserSecret GetLockingUser(SsResource resource)
        {
            throw new NotImplementedException();
        }

        public void SetLockingUser(SsResource resource, SsUserSecret user)
        {
            throw new NotImplementedException();
        }
    }
}
