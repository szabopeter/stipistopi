using ServiceInterfaces.Dto;

namespace RestApi.Controllers
{
    public class ResourceInfo
    {
        public SsResource Resource { get; set; }
        public bool IsAvailable { get; set; }
        public string LockedBy { get; set; }
    }
}
