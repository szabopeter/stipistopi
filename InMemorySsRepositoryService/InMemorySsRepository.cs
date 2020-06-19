using ServiceInterfaces;
using ServiceInterfaces.Dto;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Logic.Repository
{
    public class InMemorySsRepository : ISsRepository
    {
        public InMemorySsRepository()
        {
        }

        private static readonly object _resourceLock = new object();

        private readonly Dictionary<string, SsResource> _resources = new Dictionary<string, SsResource>();
        private readonly Dictionary<string, SsUserSecret> _users = new Dictionary<string, SsUserSecret>();
        private readonly ConcurrentDictionary<string, SsUserSecret> _usages = new ConcurrentDictionary<string, SsUserSecret>();

        public SsResource GetResource(string shortName)
        {
            if (_resources.TryGetValue(SsResource.NormalizeShortName(shortName), out var resource))
                return new SsResource(resource.ShortName, resource.Address)
                {
                    Description = resource.Description
                };
            return null;
        }

        public void SaveResource(SsResource resource)
        {
            Contract.Requires(resource != null);
            _resources[resource.ShortName] = resource;
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
            _users[user.UserName] = user;
        }

        public List<SsResource> GetResources()
        {
            lock (_resourceLock)
            {
                return _resources.Values.ToList();
            }
        }

        public IEnumerable<SsUser> GetUsers()
        {
            lock (_resourceLock)
            {
                return _users.Values.Select(user => new SsUser(user.UserName, "*", user.Role));
            }
        }

        public SsUserSecret GetLockingUser(SsResource resource)
        {
            Contract.Requires(resource != null);
            if (_usages.TryGetValue(resource.ShortName, out var lockingUser))
                return lockingUser;
            return null;
        }

        public void SetLockingUser(SsResource resource, SsUserSecret user)
        {
            Contract.Requires(resource != null);
            if (user == null)
                _usages.Remove(resource.ShortName, out var _);
            else
                _usages[resource.ShortName] = user;
        }

        public T Transaction<T>(Func<T> action)
        {
            Contract.Requires(action != null);
            lock (_resourceLock)
                return action();
        }

        public bool DeleteResource(string shortName)
        {
            _resources.Remove(SsResource.NormalizeShortName(shortName));
            return true;
        }
    }
}