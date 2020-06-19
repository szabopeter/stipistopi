using ServiceInterfaces.Dto;

namespace RestApi.Controllers
{
    public class ResourceDescriptionParameter
    {
        public string ResourceName { get; set; }
        public string OldDescription { get; set; }
        public string NewDescription { get; set; }
        public SsUser User { get; set; }
    }
}
