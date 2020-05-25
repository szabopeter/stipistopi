namespace ServiceInterfaces.Dto
{
    public class SsResource
    {
        public string ShortName { get; }
        public string Address { get; }

        public SsResource(string shortName, string address)
        {
            ShortName = NormalizeShortName(shortName);
            Address = address;
        }

        public static string NormalizeShortName(string shortName)
        {
            return shortName.ToLowerInvariant();
        }
    }
}