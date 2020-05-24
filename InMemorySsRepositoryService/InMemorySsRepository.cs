using ServiceInterfaces;
using ServiceInterfaces.Dto;
using ServiceInterfaces.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Logic.Repository
{

    public class InMemorySsRepository : ISsRepository
    {
        public SsUser AdminUser { get; }

        public InMemorySsRepository() : this(new SsUser("admin", "admin", UserRole.Admin))
        {
        }

        public InMemorySsRepository(SsUser adminUser)
        {
            Contract.Requires(adminUser != null);
            Contract.Requires(adminUser.Role == UserRole.Admin);
            AdminUser = adminUser;
            var userSecret = new SsUserSecret(adminUser);
            _users[userSecret.UserName] = userSecret;
        }

        private static readonly object _resourceLock = new object();

        private readonly Dictionary<string, SsResource> _resources = new Dictionary<string, SsResource>();
        private readonly Dictionary<string, SsUserSecret> _users = new Dictionary<string, SsUserSecret>();
        private readonly ConcurrentDictionary<SsResource, SsUserSecret> _usages = new ConcurrentDictionary<SsResource, SsUserSecret>();

        public SsResource GetResource(string shortName)
        {
            if (_resources.TryGetValue(SsResource.NormalizeShortName(shortName), out var resource))
                return resource;
            return null;
        }

        public void SaveResource(SsResource resource)
        {
            Contract.Requires(resource != null);
            _resources[SsResource.NormalizeShortName(resource.ShortName)] = resource;
        }

        public SsUserSecret GetUser(string userName)
        {
            if (_users.TryGetValue(SsUserSecret.NormalizeUserName(userName), out var user))
                return user;
            return null;
        }

        public void SaveUser(SsUserSecret user)
        {
            Contract.Requires(user != null);
            _users[SsUserSecret.NormalizeUserName(user.UserName)] = user;
        }

        public List<SsResource> GetAll()
        {
            lock (_resourceLock)
            {
                return _resources.Values.ToList();
            }
        }

        public SsUserSecret GetLockingUser(SsResource resource)
        {
            if (_usages.TryGetValue(resource, out var lockingUser))
                return lockingUser;
            return null;
        }

        public void SetLockingUser(SsResource resource, SsUserSecret user)
        {
            if (user == null)
                _usages.Remove(resource, out var _);
            else
                _usages[resource] = user;
        }

        public bool IsLocked(SsResource res)
        {
            lock (_resourceLock)
            {
                return _usages.ContainsKey(res);
            }
        }

        public SsUser GetLockedBy(SsResource res)
        {
            lock (_resourceLock)
            {
                var lockedBy = _usages.GetValueOrDefault(res, null);
                if (lockedBy == null)
                    return null;
                return new SsUser(lockedBy.UserName, null);
            }
        }

        public void Transaction(Action action)
        {
            Contract.Requires(action != null);
            lock (_resourceLock)
                action();
        }
    }
}