using Microsoft.Extensions.Localization;
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
        private const string UserDoesNotExist = "User does not exist";
        private const string PasswordDoesNotMatch = "Password does not match";
        private const string ResourceAlreadyExists = "Resource already exists";
        private const string UserAlreadyExists = "User already exists";
        private const string UnknownResource = "Unknown resource!";

        public InMemorySsRepository()
        {
            var defaultLocalizer = new DefaultStringLocalizer();
            defaultLocalizer.Add(UserDoesNotExist);
            defaultLocalizer.Add(PasswordDoesNotMatch);
            defaultLocalizer.Add(ResourceAlreadyExists);
            defaultLocalizer.Add(UserAlreadyExists);
            defaultLocalizer.Add(UnknownResource);
            Localizer = defaultLocalizer;
        }

        public InMemorySsRepository(IStringLocalizer localizer)
        {
            Contract.Requires(localizer != null);
            Localizer = localizer;
        }

        private static readonly object _resourceLock = new object();

        private readonly List<SsResource> _resources = new List<SsResource>();
        private readonly List<SsUserSecret> _users = new List<SsUserSecret>();
        private readonly ConcurrentDictionary<SsResource, SsUserSecret> _usages = new ConcurrentDictionary<SsResource, SsUserSecret>();
        private readonly IStringLocalizer Localizer;

        public void NewResource(SsResource resource)
        {
            Contract.Requires(resource != null);

            lock (_resourceLock)
            {
                if (_resources.Any(resource.IsSame))
                    throw new ArgumentException(Localizer[ResourceAlreadyExists], nameof(resource));
                _resources.Add(resource);
            }
        }

        public void NewUser(SsUser user)
        {
            Contract.Requires(user != null);
            lock (_resourceLock)
            {
                if (_users.Any(u => u.HasName(user.UserName)))
                    throw new ArgumentException(Localizer[UserAlreadyExists], nameof(user));
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
                    throw new ArgumentException(Localizer[UserDoesNotExist], nameof(user));
                if (!userSecret.IsValid(user))
                    throw new ArgumentException(Localizer[PasswordDoesNotMatch], nameof(user));
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
            Contract.Requires(resource != null);
            Contract.Requires(user != null);
            lock (_resourceLock)
            {
                if (!_resources.Contains(resource))
                    throw new ArgumentException(Localizer[UnknownResource], nameof(resource));

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