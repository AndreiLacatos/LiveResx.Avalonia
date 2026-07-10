using System;
using System.Collections.Generic;
using System.Globalization;

namespace LiveResx.Avalonia
{
    /// <summary>
    /// Provides the central entry point for managing application-wide culture switching.
    /// Use the <see cref="Instance"/> singleton to register <see cref="DynamicTranslation"/> bindings
    /// and a callback that updates the UI culture in your resource files.
    /// When <see cref="SwitchCulture"/> is called, all registered translations update automatically
    /// and data-bound UIs reflect the change.
    /// </summary>
    public sealed class DynamicLocalization
    {
        private static readonly CultureInfo StartingCulture = CultureInfo.CurrentUICulture;
        private static readonly Lazy<DynamicLocalization> _lazy = new Lazy<DynamicLocalization>(() => new DynamicLocalization());

        /// <summary>
        /// Gets the singleton <see cref="DynamicLocalization"/> instance that coordinates
        /// all dynamic translations during the application's lifetime.
        /// </summary>
        public static DynamicLocalization Instance => _lazy.Value;

        private DynamicLocalization()
        {
        }

        private readonly List<DynamicTranslation> _translations = new List<DynamicTranslation>();
        private Action<CultureInfo> _onCultureChange = _ => { };

        /// <summary>
        /// Registers a set of <see cref="DynamicTranslation"/> objects and a callback that sets
        /// the <see cref="CultureInfo"/> on your resource files. The first call also applies
        /// the current UI culture so the UI displays the correct language immediately.
        /// </summary>
        /// <param name="translations">The collection of <see cref="DynamicTranslation"/> instances
        /// to manage for culture switching.</param>
        /// <param name="updateCulture">The callback invoked with the target culture, typically
        /// used to set the <c>Culture</c> property on the generated resource Designer type.</param>
        public void Register(
            IReadOnlyList<DynamicTranslation> translations,
            Action<CultureInfo> updateCulture)
        {
            _translations.AddRange(translations);
            _onCultureChange = updateCulture;
            SwitchCulture(StartingCulture);
        }

        /// <summary>
        /// Switches the application to the specified culture. All registered translations
        /// refresh their values and the culture-change callback updates resource file cultures,
        /// enabling seamless runtime language switching.
        /// </summary>
        /// <param name="culture">The target culture to switch to.</param>
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
