using System;
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

        public void NewResource(SsResource ssResource)
        {
            SsRepository.NewResource(ssResource);
        }

        public void NewUser(SsUser user)
        {
            SsRepository.NewUser(user);
        }

        public bool LockResource(SsResource resource, SsUser user)
        {
            return SsRepository.Lock(resource, user);
        }

        public bool ReleaseResource(SsResource resource, SsUser user)
        {
            return SsRepository.Release(resource, user);
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
