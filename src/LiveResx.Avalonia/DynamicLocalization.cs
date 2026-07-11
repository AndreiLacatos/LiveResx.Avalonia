using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace LiveResx.Avalonia
{
    /// <summary>
    /// Provides the central entry point for managing application-wide locale (culture) switching.
    /// Use the <see cref="Instance"/> singleton to register <see cref="DynamicTranslation"/> bindings
    /// and a callback that updates the UI culture in your resource files.
    /// When <see cref="SwitchLocale"/> is called, <see cref="Locale"/> is updated, all registered
    /// translations refresh their values, and data-bound UIs reflect the change.
    /// </summary>
    public sealed class DynamicLocalization : INotifyPropertyChanged
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

        private CultureInfo _locale;

        /// <summary>
        /// Gets the current locale (culture). Set by calling <see cref="SwitchLocale"/>.
        /// </summary>
        public CultureInfo Locale
        {
            get => _locale;
            private set
            {
                if (Equals(_locale, value))
                    return;

                _locale = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Registers a set of <see cref="DynamicTranslation"/> objects and a callback that sets
        /// the <see cref="CultureInfo"/> on your resource files. The first call also applies
        /// the current UI culture so the UI displays the correct language immediately.
        /// </summary>
        /// <param name="translations">The collection of <see cref="DynamicTranslation"/> instances
        /// to manage for locale switching.</param>
        /// <param name="updateCulture">The callback invoked with the target culture, typically
        /// used to set the <c>Culture</c> property on the generated resource Designer type.</param>
        public void Register(
            IReadOnlyList<DynamicTranslation> translations,
            Action<CultureInfo> updateCulture)
        {
            _translations.AddRange(translations);
            _onCultureChange = updateCulture;
            SwitchLocale(StartingCulture);
        }

        /// <summary>
        /// Switches the application to the specified locale. <see cref="Locale"/> is updated,
        /// the culture-change callback is invoked, and all registered translations refresh their
        /// values so the UI updates automatically.
        /// </summary>
        /// <param name="culture">The target locale (culture) to switch to.</param>
        public void SwitchLocale(CultureInfo culture)
        {
            Locale = culture;
            _onCultureChange(culture);
            foreach (var t in _translations)
            {
                t.Refresh(culture);
            }
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
