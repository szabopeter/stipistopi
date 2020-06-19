using ServiceInterfaces.Dto;

namespace RestApi.Controllers
{
    public class UserAndUserParameter
    {
        public SsUser User { get; set; }
        public SsUser Creator { get; set; }
    }
}
