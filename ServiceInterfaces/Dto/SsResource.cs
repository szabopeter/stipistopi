using System.Diagnostics.Contracts;

namespace ServiceInterfaces.Dto
{
    public class SsResource
    {
        public string ShortName
        {
            get => shortName;
            set => shortName = NormalizeShortName(value);
        }

        private string shortName;

        public string Address { get; set; }

        /// <summary>For serialization</summary>
        public SsResource()
        {
        }

        public SsResource(string shortName, string address)
        {
            ShortName = shortName;
            Address = address;
        }

        public static string NormalizeShortName(string shortName)
        {
            Contract.Requires(shortName != null);
            return shortName.ToUpperInvariant();
        }
    }
}