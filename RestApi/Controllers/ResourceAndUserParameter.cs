using ServiceInterfaces.Dto;

namespace RestApi.Controllers
{
    public class ResourceAndUserParameter
    {
        public SsResource Resource { get; set; }
        public SsUser Creator { get; set; }
    }
}
