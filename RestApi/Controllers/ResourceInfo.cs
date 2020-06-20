using System;
using ServiceInterfaces.Dto;

namespace RestApi.Controllers
{
    public class ResourceInfo
    {
        public SsResource Resource { get; set; }
        public LockingInfo Locking { get; set; }
        public bool IsAvailable { get; set; }
        [Obsolete]
        public string LockedBy { get; set; }
    }
}
