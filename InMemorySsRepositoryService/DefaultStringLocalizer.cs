using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Globalization;

namespace Logic.Repository
{
    class DefaultStringLocalizer : IStringLocalizer
    {
        public LocalizedString this[string name] => Localizations[name];

        public LocalizedString this[string name, params object[] arguments] => this[name];

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => Localizations.Values;

        public IStringLocalizer WithCulture(CultureInfo culture) => this;

        private Dictionary<string, LocalizedString> Localizations { get; } = new Dictionary<string, LocalizedString>();

        public void Add(string name, string value=null)
        {
            Localizations[name] = new LocalizedString(name, value ?? name);
        }
    }
}