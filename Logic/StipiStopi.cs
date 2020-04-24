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

        public bool LockResource(SsResource resource)
        {
            return false;
        }

        public void NewResource(SsResource ssResource)
        {
            SsRepository.Add(ssResource);
        }

        public void ReleaseResource(SsResource resource)
        {

        }
    }
}
