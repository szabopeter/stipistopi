using ServiceInterfaces.Dto;

namespace RestApi.Controllers
{
    public class DbImportParameter
    {
        public SsUser User { get; set; }
        public string Content { get; set; }
    }
}