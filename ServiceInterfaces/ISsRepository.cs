using ServiceInterfaces.Dto;
using System;
using System.Collections.Generic;

namespace ServiceInterfaces
{
    public interface ISsRepository
    {
        List<SsResource> GetAll();
        bool Lock(SsResource resource, SsUserSecret user);
        bool IsLocked(SsResource res);
        bool Release(SsResource resource, SsUserSecret user);
        SsUser GetLockedBy(SsResource ssr);
        void Transaction(Action action);
        SsResource GetResource(string shortName);
        void SaveResource(SsResource resource);
        SsUserSecret GetUser(string userName);
        void SaveUser(SsUserSecret user);
    }
}