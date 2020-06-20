using System;

namespace ServiceInterfaces.Dto
{
    public class LockingInfo
    {
        public SsResource Resource { get; set; }
        public SsUser LockedBy { get; set; }
        public DateTime? LockedAt { get; set; }
        public string Comment { get; set; }
    }
}