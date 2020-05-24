using ServiceInterfaces.Dto;
using System;
using System.Collections.Generic;

namespace ServiceInterfaces
{
    public interface ISsRepository
    {
        List<SsResource> GetAll();
        SsUser GetLockedBy(SsResource ssr);
        void Transaction(Action action);
        SsResource GetResource(string shortName);
        void SaveResource(SsResource resource);
        SsUserSecret GetUser(string userName);
        void SaveUser(SsUserSecret user);
        SsUserSecret GetLockingUser(SsResource resource);
        void SetLockingUser(SsResource resource, SsUserSecret user);
    }
}