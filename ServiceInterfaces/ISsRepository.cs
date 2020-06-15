using ServiceInterfaces.Dto;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace ServiceInterfaces
{
    public interface ISsRepository
    {
        List<SsResource> GetResources();
        T Transaction<T>(Func<T> action);
        SsResource GetResource(string shortName);
        void SaveResource(SsResource resource);
        SsUserSecret GetUser(string userName);
        void SaveUser(SsUserSecret user);
        SsUserSecret GetLockingUser(SsResource resource);
        void SetLockingUser(SsResource resource, SsUserSecret user);
        IEnumerable<SsUser> GetUsers();
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