using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace LiveResx.Avalonia
{
    /// <summary>
    /// Represents a culture-aware resource of type <typeparamref name="T"/> that supports
    /// data-binding through <see cref="INotifyPropertyChanged"/>. Register an instance with
    /// <see cref="DynamicLocalization.RegisterResource{T}"/> to have it automatically refreshed
    /// when <see cref="DynamicLocalization.SwitchLocale"/> is called.
    /// </summary>
    /// <typeparam name="T">The type of the resource value.</typeparam>
    public sealed class LocalizedResource<T> : IRefreshableResource, INotifyPropertyChanged
    {
        private readonly Dictionary<CultureInfo, T> _values;
        private readonly string _name;
        private readonly FallbackBehavior _fallback;
        private T _current;

        /// <summary>
        /// Creates a new culture-aware resource with the specified values.
        /// </summary>
        /// <param name="name">The unique name used to identify this resource.
        /// Must not be <c>null</c> or consist only of whitespace.</param>
        /// <param name="values">A dictionary mapping each culture to its corresponding value.
        /// Must not be <c>null</c>.</param>
        /// <param name="fallback">The fallback strategy to use when no exact culture match exists.
        /// Defaults to <see cref="FallbackBehavior.Invariant"/>.</param>
        /// <exception cref="ArgumentException"><paramref name="name"/> is <c>null</c>
        /// or consists only of whitespace.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="values"/> is <c>null</c>.</exception>
        public LocalizedResource(
            string name,
            Dictionary<CultureInfo, T> values,
            FallbackBehavior fallback = FallbackBehavior.Invariant)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name must not be empty", nameof(name));
            }

            _name = name;
            _values = values ?? throw new ArgumentNullException(nameof(values));
            _fallback = fallback;
        }

        /// <summary>
        /// Gets the unique name used to identify this resource.
        /// </summary>
        public string Name => _name;

        /// <summary>
        /// Gets the current value for the active culture. This property is updated
        /// automatically when <see cref="DynamicLocalization.SwitchLocale"/> is called.
        /// Raised <see cref="PropertyChanged"/> when the value changes.
        /// </summary>
        public T Value
        {
            get => _current;
            private set
            {
                if (EqualityComparer<T>.Default.Equals(_current, value))
                    return;

                _current = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Refreshes the resource value for the specified culture. This is called
        /// automatically by <see cref="DynamicLocalization.SwitchLocale"/> for registered resources.
        /// </summary>
        /// <param name="culture">The culture to resolve the value for.</param>
        internal void Refresh(CultureInfo culture)
        {
            Value = ResolveValue(culture);
        }

        void IRefreshableResource.Refresh(CultureInfo culture)
        {
            Refresh(culture);
        }

        private T ResolveValue(CultureInfo culture)
        {
            // Attempt exact culture match
            if (culture != null && _values.TryGetValue(culture, out var exact))
            {
                return exact;
            }

            // Walk parent chain if ParentChain mode and culture is available
            if (_fallback == FallbackBehavior.ParentChain && culture != null)
            {
                var parent = culture.Parent;
                while (parent != CultureInfo.InvariantCulture)
                {
                    if (_values.TryGetValue(parent, out var neutral))
                    {
                        return neutral;
                    }

                    parent = parent.Parent;
                }
            }

            // Try invariant culture entry
            if (_values.TryGetValue(CultureInfo.InvariantCulture, out var invariant))
            {
                return invariant;
            }

            // Fallback to first available entry
            foreach (var kvp in _values)
            {
                return kvp.Value;
            }

            return default(T);
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
