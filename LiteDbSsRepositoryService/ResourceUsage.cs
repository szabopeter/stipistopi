using System;

namespace LiteDbSsRepositoryService
{
    public class ResourceUsage
    {
        public string ResourceShortName { get; set; }
        public string UserName { get; set; }
        public DateTime LockedAt { get; set; }
        public string Comment { get; set; }
    }
}
