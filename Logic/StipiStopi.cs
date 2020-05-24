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
                var testuser = new SsUser("test", "test");
                SsRepository.NewUser(testuser);
                var ncu139 = new SsResource("ncu1", "10.10.148.8");
                SsRepository.NewResource(ncu139);
                var ncu140 = new SsResource("ncu2", "10.10.148.9");
                SsRepository.NewResource(ncu140);
                SsRepository.Lock(ncu140, SsRepository.Authenticated(testuser));
            });
        }

        public List<SsResource> GetResources()
        {
            return SsRepository.GetAll();
        }

        public SsResource NewResource(string shortName, string address, SsUser user)
        {
            // TODO why not pass an SsResource?
            var ssResource = new SsResource(shortName, address);
            SsRepository.Transaction(() => 
            {
                if (Authenticated(user).Role != UserRole.Admin)
                    // TODO Create new Exception type
                    throw new UserDoesNotExistException(user.UserName);
                SsRepository.NewResource(ssResource);
            });
            return ssResource;
        }

        public SsUser NewUser(string username, string password)
        {
            var user = new SsUser(username, password);
            SsRepository.NewUser(user);
            return user;
        }

        public SsUser GetLockedBy(SsResource ssr)
        {
            return SsRepository.GetLockedBy(ssr);
        }

        private SsUserSecret Authenticated(SsUser user)
        {
            return SsRepository.Authenticated(user);
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
