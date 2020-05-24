using System;
using System.Collections.Generic;
using ServiceInterfaces;
using ServiceInterfaces.Dto;

namespace LiteDbSsRepositoryService
{
    public class LiteDbSsRepository : ISsRepository
    {
        public SsUserSecret Authenticated(SsUser user)
        {
            throw new NotImplementedException();
        }

        public List<SsResource> GetAll()
        {
            throw new NotImplementedException();
        }

        public SsUser GetLockedBy(SsResource ssr)
        {
            throw new NotImplementedException();
        }

        public bool IsLocked(SsResource res)
        {
            throw new NotImplementedException();
        }

        public bool Lock(SsResource resource, SsUserSecret user)
        {
            throw new NotImplementedException();
        }

        public void NewResource(SsResource ssResource)
        {
            throw new NotImplementedException();
        }

        public void NewUser(SsUser user)
        {
            throw new NotImplementedException();
        }

        public bool Release(SsResource resource, SsUserSecret user)
        {
            throw new NotImplementedException();
        }

        public void Transaction(Action action)
        {
            throw new NotImplementedException();
        }
    }
}
