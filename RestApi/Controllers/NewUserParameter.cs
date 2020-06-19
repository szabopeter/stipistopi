using ServiceInterfaces.Dto;

namespace RestApi.Controllers
{
    public class NewUserParameter
    {
        public SsUser User { get; set; }
        public SsUser Creator { get; set; }
    }
}
