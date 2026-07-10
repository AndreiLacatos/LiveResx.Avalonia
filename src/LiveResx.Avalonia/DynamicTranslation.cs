using System;
using System.ComponentModel;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace LiveResx.Avalonia
{
    /// <summary>
    /// Represents a data-bindable binding between a <c>.resx</c> resource key and a
    /// <see cref="ResourceManager"/>. Wires into Avalonia's data-binding system via
    /// <see cref="INotifyPropertyChanged"/> so the UI automatically updates when the culture
    /// changes through <see cref="DynamicLocalization.SwitchCulture"/>.
    /// </summary>
    public sealed class DynamicTranslation : INotifyPropertyChanged
    {
        private readonly string _resourceKey;
        private readonly Lazy<ResourceManager> _resourceManager;
        private CultureInfo _culture;

        /// <summary>
        /// Creates a translation that resolves the specified <paramref name="resourceKey"/>
        /// at runtime using the provided <paramref name="resourceManagerFactory"/>.
        /// The factory is called lazily on first access to <see cref="Text"/>,
        /// avoiding eager ResourceManager creation at static initialization time.
        /// The resource key must not be <c>null</c> or whitespace.
        /// </summary>
        /// <param name="resourceKey">The key of the resource string to look up. Must not be
        /// <c>null</c> or consist only of whitespace.</param>
        /// <param name="resourceManagerFactory">A factory function that returns the
        /// <see cref="ResourceManager"/> for the target resource file. Must not be <c>null</c>.</param>
        /// <exception cref="ArgumentException"><paramref name="resourceKey"/> is <c>null</c>
        /// or consists only of whitespace.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="resourceManagerFactory"/> is <c>null</c>.</exception>
        public DynamicTranslation(
            string resourceKey,
            Func<ResourceManager> resourceManagerFactory)
        {
            if (string.IsNullOrWhiteSpace(resourceKey))
            {
                throw new ArgumentException("Resource key must not be empty", nameof(resourceKey));
            }

            _resourceKey = resourceKey;
            _resourceManager = new Lazy<ResourceManager>(
                resourceManagerFactory ?? throw new ArgumentNullException(nameof(resourceManagerFactory)));
        }

        /// <summary>
        /// Gets the localized string for the current culture by looking up the resource key
        /// via the <see cref="ResourceManager"/>. This is the property you data-bind to in XAML;
        /// it automatically raises <see cref="PropertyChanged"/> when the culture switches.
        /// <para />
        /// The <see cref="ResourceManager"/> is obtained from the factory on the first access
        /// and cached for the lifetime of this translation.
        /// </summary>
        /// <returns>The localized string, or <see cref="string.Empty"/> if the resource key
        /// is not found.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="ResourceManager"/>
        /// has not been initialized.</exception>
        /// <exception cref="InvalidOperationException">The culture has not been set.
        /// Call <see cref="DynamicLocalization.SwitchCulture"/> to set the active culture.</exception>
        public string Text
        {
            get
            {
                var rm = _resourceManager.Value;
                if (rm is null)
                {
                    throw new InvalidOperationException("Resource manager is not initialized");
                }
                if (_culture is null)
                {
                    throw new InvalidOperationException("Culture not set");
                }

                return rm.GetString(_resourceKey, _culture) ?? string.Empty;
            }
        }

        internal void Refresh(CultureInfo culture)
        {
            _culture = culture;
            OnPropertyChanged(nameof(Text));
        }

        /// <summary>
        /// Raised when <see cref="Text"/> changes due to a culture switch.
        /// Avalonia data-binding listens to this event to update the UI automatically.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
