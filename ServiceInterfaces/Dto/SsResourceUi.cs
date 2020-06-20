using System.Globalization;

namespace ServiceInterfaces.Dto
{
    public class SsResourceUi
    {
        private readonly SsResource resource;

        public SsResourceUi()
        {
        }

        public SsResourceUi(SsResource resource)
        {
            this.resource = resource;
        }

        public string LockedAt => resource.Locking?.LockedAt?.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
    }
}