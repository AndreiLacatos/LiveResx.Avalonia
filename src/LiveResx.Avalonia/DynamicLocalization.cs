using System;
using System.Collections.Generic;
using System.Globalization;

namespace LiveResx.Avalonia
{
    public sealed class DynamicLocalization
    {
        private static readonly CultureInfo StartingCulture = CultureInfo.CurrentUICulture;
        private static DynamicLocalization _instance;
        public static DynamicLocalization Instance => _instance ?? (_instance = new DynamicLocalization());

        private DynamicLocalization()
        {
        }

        private readonly List<DynamicTranslation> _translations = new List<DynamicTranslation>();
        private Action<CultureInfo> _onCultureChange = _ => { };

        public void Register(
            IReadOnlyList<DynamicTranslation> translations,
            Action<CultureInfo> updateCulture)
        {
            _translations.AddRange(translations);
            _onCultureChange = updateCulture;
            SwitchCulture(StartingCulture);
        }

        public void SwitchCulture(CultureInfo culture)
        {
            _onCultureChange(culture);
            foreach (var t in _translations)
            {
                t.Refresh(culture);
            }
        }
    }
}
