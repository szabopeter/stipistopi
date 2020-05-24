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
                SsRepository.Lock(ncu140, testuser);
            });
        }

        public List<SsResource> GetResources()
        {
            return SsRepository.GetAll();
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
            return SsRepository.GetLockedBy(ssr);
        }

        private SsUserSecret Authenticated(SsUser user)
        {
            SsUserSecret authenticated = null;
            SsRepository.Transaction(() =>
            {
                var userSecret = SsRepository.GetUser(user.UserName);
                if (userSecret == null)
                    throw new UserDoesNotExistException(user.UserName);

                if (!userSecret.IsValid(user))
                    throw new InvalidPasswordException(user.UserName);
                
                authenticated = userSecret;
            });

            return authenticated;
        }

        public bool LockResource(SsResource resource, SsUser user)
        {
            return SsRepository.Lock(resource, Authenticated(user));
        }

        public bool ReleaseResource(SsResource resource, SsUser user)
        {
            return SsRepository.Release(resource, Authenticated(user));
        }

        public bool IsLocked(SsResource res)
        {
            return SsRepository.IsLocked(res);
        }

        public bool IsFree(SsResource res)
        {
            return !IsLocked(res);
        }
    }
}
