using ServiceInterfaces.Dto;
using System;
using System.Collections.Generic;

namespace ServiceInterfaces
{
    public interface ISsRepository
    {
        void NewResource(SsResource ssResource);
        List<SsResource> GetAll();
        bool Lock(SsResource resource, SsUserSecret user);
        void NewUser(SsUser user);
        bool IsLocked(SsResource res);
        bool Release(SsResource resource, SsUserSecret user);
        SsUserSecret Authenticated(SsUser user);
        SsUser GetLockedBy(SsResource ssr);
        void Transaction(Action action);
    }
}