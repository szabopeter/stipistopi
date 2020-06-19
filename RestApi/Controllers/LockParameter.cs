using ServiceInterfaces.Dto;

namespace RestApi.Controllers
{
    public class LockParameter
    {
        public SsUser User { get; set; }
        public string ResourceName { get; set; }
    }
}
