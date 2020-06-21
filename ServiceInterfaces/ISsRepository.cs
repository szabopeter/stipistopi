using ServiceInterfaces.Dto;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace ServiceInterfaces
{
    public interface ISsRepository
    {
        // TODO: GetLockingInfos
        List<SsResource> GetResources();
        T Transaction<T>(Func<T> action);
        SsResource GetResource(string shortName);
        void SaveResource(SsResource resource);
        SsUserSecret GetUser(string userName);
        void SaveUser(SsUserSecret user);
        LockingInfo GetLocking(SsResource resource);
        void Release(SsResource resource);
        void Lock(SsResource resource, string userName, string comment);
        IEnumerable<SsUser> GetUsers();
        bool DeleteResource(string shortName);
        bool DeleteUser(string userName);
        bool DbImport(string content);
        string DbExport();
    }

    public static class ISsRepositoryExtensions
    {
        public static void Transaction(this ISsRepository repository, Action action)
        {
            Contract.Requires(repository != null);
            Contract.Requires(action != null);

            repository.Transaction(() =>
            {
                action();
                return 0;
            });
        }
    }
}