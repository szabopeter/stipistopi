using ServiceInterfaces.Dto;
using System;
using System.Collections.Generic;

namespace ServiceInterfaces
{
    public interface ISsRepository
    {
        List<SsResource> GetAll();
        void Transaction(Action action);
        // TODO: Create a Transaction<T>(Func<T> action) alternative
        SsResource GetResource(string shortName);
        void SaveResource(SsResource resource);
        SsUserSecret GetUser(string userName);
        void SaveUser(SsUserSecret user);
        SsUserSecret GetLockingUser(SsResource resource);
        void SetLockingUser(SsResource resource, SsUserSecret user);
    }
}