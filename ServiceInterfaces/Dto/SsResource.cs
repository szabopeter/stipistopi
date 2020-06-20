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

        public string Description
        {
            get => description ?? "";
            set => description = value;
        }

        private string description;

        public LockingInfo Locking { get; set; }

        public SsResourceUi Ui { get; private set; }
        public void LoadUiProperty() => Ui = new SsResourceUi(this);

        /// <summary>For serialization</summary>
        public SsResource()
        {
        }

        public SsResource(string shortName, string address)
        {
            ShortName = shortName;
            Address = address;
            Description = "";
        }

        public static string NormalizeShortName(string shortName)
        {
            Contract.Requires(shortName != null);
            return shortName.ToUpperInvariant();
        }
    }
}