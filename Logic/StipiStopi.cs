using System.Collections.Generic;
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
                var testuser = new SsUserSecret(new SsUser("test", "test"));
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
            List<SsResource> resources = null;
            SsRepository.Transaction(() => 
                resources = SsRepository.GetAll());
            return resources;
        }

        public SsResource NewResource(string shortName, string address, SsUser creator)
        {
            // TODO why not pass an SsResource?
            var ssResource = new SsResource(shortName, address);
            SsRepository.Transaction(() =>
            {
                if (Authenticated(creator)?.Role != UserRole.Admin)
                    throw new InsufficientRoleException(creator.UserName);

                if (SsRepository.GetResource(shortName) != null)
                    throw new ResourceAlreadyExistsException(shortName);

                SsRepository.SaveResource(ssResource);
            });
            return ssResource;
        }

        public SsUser NewUser(string username, string password, SsUser creator)
        {
            var user = new SsUser(username, password);
            SsRepository.Transaction(() =>
            {
                if (Authenticated(creator)?.Role != UserRole.Admin)
                    throw new InsufficientRoleException(creator.UserName);

                if (SsRepository.GetUser(username) != null)
                    throw new UserAlreadyExistsException(username);

                SsRepository.SaveUser(new SsUserSecret(user));
            });
            return user;
        }

        public SsUser GetLockedBy(SsResource ssr)
        {
            // TODO Returning the username only should be sufficient
            // TODO Request should require a valid user
            SsUser lockingUser = null;
            SsRepository.Transaction(() => {
                var user = SsRepository.GetLockingUser(ssr);
                if (user != null)
                    lockingUser = new SsUser(user.UserName, "", user.Role);
            });
            return lockingUser;
        }

        private SsUserSecret Authenticated(SsUser user)
        {
            SsUserSecret authenticated = null;
            // SsRepository.Transaction(() =>
            // {
                var userSecret = SsRepository.GetUser(user.UserName);
                if (userSecret == null)
                    throw new UserDoesNotExistException(user.UserName);

                if (!userSecret.IsValid(user))
                    throw new InvalidPasswordException(user.UserName);
                
                authenticated = userSecret;
            // });

            return authenticated;
        }

        public bool LockResource(SsResource resource, SsUser user)
        {
            var shortName = resource?.ShortName;
            // TODO shortName would be sufficient
            bool success = false;
            SsRepository.Transaction(() => {
                var authenticated = Authenticated(user);

                // TODO Should extract a GetResource
                SsResource dbResource = SsRepository.GetResource(shortName);
                if (dbResource == null)
                    throw new ResourceDoesNotExistException(shortName);
                
                var lockedBy = SsRepository.GetLockingUser(dbResource);
                if (lockedBy != null)
                    success = false;
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
            SsRepository.Transaction(() => {
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
                } else {
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
