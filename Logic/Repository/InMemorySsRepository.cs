using Logic.Dto;
using Logic.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Logic.Repository
{

    public class InMemorySsRepository : ISsRepository
    {
        private static readonly object _resourceLock = new object();

        private readonly List<SsResource> _resources = new List<SsResource>();
        private readonly List<SsUserSecret> _users = new List<SsUserSecret>();
        private readonly ConcurrentDictionary<SsResource, SsUserSecret> _usages = new ConcurrentDictionary<SsResource, SsUserSecret>();

        public void NewResource(SsResource resource)
        {
            lock (_resourceLock)
            {
                if (_resources.Any(resource.IsSame))
                    throw new ArgumentException("Resource already exists", nameof(resource));
                _resources.Add(resource);
            }
        }

        public void NewUser(SsUser user)
        {
            lock (_resourceLock)
            {
                if (_users.Any(u => u.HasName(user.UserName)))
                    throw new ArgumentException("User already exists", nameof(user));
                var userSecret = new SsUserSecret(user);
                _users.Add(userSecret);
            }
        }

        public SsUserSecret Authenticated(SsUser user)
        {
            lock (_resourceLock)
            {
                var userSecret = _users.SingleOrDefault(u => u.HasName(user.UserName));
                if (userSecret == null)
                    throw new ArgumentException("User does not exist", nameof(user.UserName));
                if (!userSecret.IsValid(user))
                    throw new ArgumentException("Password does not match", nameof(user.Password));
                return userSecret;
            }
        }

        public List<SsResource> GetAll()
        {
            lock (_resourceLock)
            {
                return _resources.ToList();
            }
        }

        public bool Lock(SsResource resource, SsUserSecret user)
        {
            lock (_resourceLock)
            {
                if (!_resources.Contains(resource))
                    throw new ArgumentException("Unknown resource!", nameof(resource));

                if (_usages.ContainsKey(resource))
                    return false;

                _usages[resource] = user;
                return true;
            }
        }

        public bool Release(SsResource resource, SsUserSecret user)
        {
            lock (_resourceLock)
            {
                if (_usages[resource] != user)
                    return false;

                _usages.Remove(resource, out var _);
                return true;
            }
        }

        public bool IsLocked(SsResource res)
        {
            lock (_resourceLock)
            {
                return _usages.ContainsKey(res);
            }
        }
    }
}