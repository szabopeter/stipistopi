using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using ServiceInterfaces;
using ServiceInterfaces.Dto;
using ServiceInterfaces.Exceptions;

namespace logic
{
    public class StipiStopi
    {
        private ISsRepository SsRepository { get; }

        public StipiStopi(ISsRepository ssRepository)
        {
            SsRepository = ssRepository;
        }

        public void Populate()
        {
            SsRepository.Transaction(() =>
            {
                var testuser = new SsUserSecret(new SsUser("test", "test", UserRole.Admin));
                SsRepository.SaveUser(testuser);
                var ncu139 = new SsResource("ncu1", "10.10.148.8");
                SsRepository.SaveResource(ncu139);
                var ncu140 = new SsResource("ncu2", "10.10.148.9");
                SsRepository.SaveResource(ncu140);
                SsRepository.SetLockingUser(ncu140, testuser);
            });
        }

        public List<SsResource> GetResources()
        {
            // TODO : only for Authenticated users!
            List<SsResource> resources = null;
            SsRepository.Transaction(() =>
                resources = SsRepository.GetResources());
            return resources;
        }

        public IEnumerable<SsUser> GetUsers(SsUser user)
        {
            IEnumerable<SsUser> users = new SsUser[0];
            SsRepository.Transaction(() => {
                if (Authenticated(user) == null)
                    return;

                users = SsRepository.GetUsers();
            });
            return users;
        }

        public SsResource NewResource(SsResource ssResource, SsUser creator)
        {
            Contract.Requires(ssResource != null);
            SsRepository.Transaction(() =>
            {
                if (Authenticated(creator)?.Role != UserRole.Admin)
                    throw new InsufficientRoleException(creator.UserName);

                if (SsRepository.GetResource(ssResource.ShortName) != null)
                    throw new ResourceAlreadyExistsException(ssResource.ShortName);

                SsRepository.SaveResource(ssResource);
            });
            return ssResource;
        }

        public SsUser NewUser(SsUser newUser, SsUser creator)
        {
            SsRepository.Transaction(() =>
            {
                if (Authenticated(creator)?.Role != UserRole.Admin)
                    throw new InsufficientRoleException(creator.UserName);

                if (SsRepository.GetUser(newUser.UserName) != null)
                    throw new UserAlreadyExistsException(newUser.UserName);

                SsRepository.SaveUser(new SsUserSecret(newUser));
            });
            return newUser;
        }

        public SsUser GetLockedBy(SsResource ssr)
        {
            // TODO Returning the username only should be sufficient
            // TODO Request should require a valid user
            SsUser lockingUser = null;
            SsRepository.Transaction(() =>
            {
                var user = SsRepository.GetLockingUser(ssr);
                if (user != null)
                    lockingUser = new SsUser(user.UserName, "", user.Role);
            });
            return lockingUser;
        }

        private SsUserSecret Authenticated(SsUser user)
        {
            var userSecret = SsRepository.GetUser(user.UserName);
            if (userSecret == null)
                throw new UserDoesNotExistException(user.UserName);

            if (!userSecret.IsValid(user))
                throw new InvalidPasswordException(user.UserName);

            return userSecret;
        }

        public bool LockResource(SsResource resource, SsUser user)
        {
            var shortName = resource?.ShortName;
            // TODO shortName would be sufficient
            bool success = false;
            SsRepository.Transaction(() =>
            {
                var authenticated = Authenticated(user);

                // TODO Should extract a GetResource
                SsResource dbResource = SsRepository.GetResource(shortName);
                if (dbResource == null)
                    throw new ResourceDoesNotExistException(shortName);

                var lockedBy = SsRepository.GetLockingUser(dbResource);
                if (lockedBy != null)
                {
                    success = false;
                }
                else
                {
                    SsRepository.SetLockingUser(dbResource, authenticated);
                    success = true;
                }
            });
            return success;
        }

        public bool ReleaseResource(SsResource resource, SsUser user)
        {
            var shortName = resource?.ShortName;
            // TODO shortname should be sufficient
            bool success = false;
            SsRepository.Transaction(() =>
            {
                var authenticated = Authenticated(user);

                // TODO Should extract a GetResource
                SsResource dbResource = SsRepository.GetResource(shortName);
                if (dbResource == null)
                    throw new ResourceDoesNotExistException(shortName);

                var lockedBy = SsRepository.GetLockingUser(dbResource);
                if (lockedBy.Equals(authenticated))
                {
                    SsRepository.SetLockingUser(dbResource, null);
                    success = true;
                }
                else
                {
                    success = false;
                }
            });
            return success;
        }

        public bool IsLocked(SsResource res)
        {
            // TODO: this operation should require a valid user
            bool result = false;
            SsRepository.Transaction(() => result = SsRepository.GetLockingUser(res) != null);
            return result;
        }

        public bool IsFree(SsResource res)
        {
            return !IsLocked(res);
        }
    }
}
