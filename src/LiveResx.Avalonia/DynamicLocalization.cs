using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace LiveResx.Avalonia
{
    /// <summary>
    /// Internal interface that allows <see cref="DynamicLocalization"/> to refresh all
    /// registered custom resources without knowing their concrete type.
    /// </summary>
    internal interface ILocalizedResource
    {
        string Name { get; }
        void Refresh(CultureInfo culture);
    }

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
        private readonly Dictionary<string, ILocalizedResource> _customResources = new Dictionary<string, ILocalizedResource>();
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
            foreach (var r in _customResources.Values)
            {
                r.Refresh(culture);
            }
        }

        /// <summary>
        /// Registers a custom typed resource that will be refreshed automatically when
        /// <see cref="SwitchLocale"/> is called. The resource is immediately refreshed
        /// with the current locale.
        /// </summary>
        /// <typeparam name="T">The type of the resource value.</typeparam>
        /// <param name="resource">The resource to register. Must not be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="resource"/> is <c>null</c>.</exception>
        public void RegisterResource<T>(LocalizedResource<T> resource)
        {
            if (resource is null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            _customResources[resource.Name] = resource;
            resource.Refresh(Locale);
        }

        /// <summary>
        /// Gets a previously registered custom resource by name.
        /// </summary>
        /// <typeparam name="T">The type of the resource value.</typeparam>
        /// <param name="name">The name of the resource to retrieve.</param>
        /// <returns>The registered <see cref="LocalizedResource{T}"/> instance.</returns>
        /// <exception cref="KeyNotFoundException">No resource with the specified name is registered.</exception>
        /// <exception cref="InvalidCastException">The registered resource's type does not match <typeparamref name="T"/>.</exception>
        public LocalizedResource<T> GetResource<T>(string name)
        {
            return (LocalizedResource<T>)_customResources[name];
        }

        /// <summary>
        /// Tries to get a previously registered custom resource by name.
        /// Returns <c>true</c> if the resource is found and the type matches;
        /// otherwise <c>false</c>.
        /// </summary>
        /// <typeparam name="T">The type of the resource value.</typeparam>
        /// <param name="name">The name of the resource to retrieve.</param>
        /// <param name="resource">When this method returns, contains the registered resource
        /// if found and type-compatible; otherwise <c>null</c>.</param>
        /// <returns><c>true</c> if the resource was found and type-compatible; otherwise <c>false</c>.</returns>
        public bool TryGetResource<T>(string name, out LocalizedResource<T> resource)
        {
            if (_customResources.TryGetValue(name, out var raw) && raw is LocalizedResource<T> typed)
            {
                resource = typed;
                return true;
            }

            resource = null;
            return false;
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
