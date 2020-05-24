using ServiceInterfaces;
using ServiceInterfaces.Dto;
using ServiceInterfaces.Exceptions;
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
            _users.Add(userSecret);
        }

        private static readonly object _resourceLock = new object();

        private readonly List<SsResource> _resources = new List<SsResource>();
        private readonly List<SsUserSecret> _users = new List<SsUserSecret>();
        private readonly ConcurrentDictionary<SsResource, SsUserSecret> _usages = new ConcurrentDictionary<SsResource, SsUserSecret>();

        public void NewResource(SsResource resource)
        {
            Contract.Requires(resource != null);

            lock (_resourceLock)
            {
                if (_resources.Any(resource.IsSame))
                    throw new ResourceAlreadyExistsException(resource.ShortName);
                _resources.Add(resource);
            }
        }

        public void NewUser(SsUser user)
        {
            Contract.Requires(user != null);
            lock (_resourceLock)
            {
                if (_users.Any(u => u.HasName(user.UserName)))
                    throw new UserAlreadyExistsException(user.UserName);
                var userSecret = new SsUserSecret(user);
                _users.Add(userSecret);
            }
        }

        public SsUserSecret Authenticated(SsUser user)
        {
            Contract.Requires(user != null);
            lock (_resourceLock)
            {
                var userSecret = _users.SingleOrDefault(u => u.HasName(user.UserName));
                if (userSecret == null)
                    throw new UserDoesNotExistException(user.UserName);
                if (!userSecret.IsValid(user))
                    throw new InvalidPasswordException(user.UserName);
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
            // TODO authentication needs to happen inside the locked block!
            Contract.Requires(resource != null);
            Contract.Requires(user != null);
            lock (_resourceLock)
            {
                if (!_resources.Contains(resource))
                    throw new ResourceDoesNotExistException(resource?.ShortName);

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
    }
}