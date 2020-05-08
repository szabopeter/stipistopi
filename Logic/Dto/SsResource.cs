namespace Logic.Dto
{
    public class SsResource
    {
        public string ShortName { get; }
        public string Address { get; }

        public SsResource(string shortName, string address)
        {
            ShortName = shortName;
            Address = address;
        }

        public bool IsSame(SsResource other)
        {
            return string.Equals(ShortName, other.ShortName, System.StringComparison.InvariantCultureIgnoreCase);
        }
    }
}