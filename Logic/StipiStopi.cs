using System.Collections.Generic;

namespace logic
{
    public class StipiStopi
    {
        public StipiStopi(ISsRepository ssRepository)
        {
            SsRepository = ssRepository;
        }

        public ISsRepository SsRepository { get; }

        public List<SsResource> GetResources()
        {
            return SsRepository.GetAll();
        }

        public SsResource NewResource(string shortName, string address)
        {
            var ssResource = new SsResource(shortName, address);
            SsRepository.NewResource(ssResource);
            return ssResource;
        }

        public SsUser NewUser(string username, string password)
        {
            var user = new SsUser(username, password);
            SsRepository.NewUser(user);
            return user;
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
