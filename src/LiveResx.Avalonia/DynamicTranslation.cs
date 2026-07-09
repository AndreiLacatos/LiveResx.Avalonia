using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace LiveResx.Avalonia
{
    public sealed class DynamicTranslation : INotifyPropertyChanged
    {
        private readonly string _resourceKey;
        private readonly ResourceManager _resourceManager;
        private CultureInfo _culture;

        public DynamicTranslation(
            string resourceKey,
            Func<ResourceManager> resourceManagerFactory)
        {
            if (string.IsNullOrWhiteSpace(resourceKey))
            {
                throw new ArgumentException("Resource key must not be empty", nameof(resourceKey));
            }

            _resourceKey = resourceKey;
            _resourceManager = resourceManagerFactory();
        }

        public string Text
        {
            get
            {
                if (_resourceManager is null)
                {
                    throw new InvalidOperationException("Resource manager is not initialized");
                }
                if (_culture is null)
                {
                    throw new InvalidOperationException("Culture not set");
                }

                return _resourceManager.GetString(_resourceKey, _culture) ?? string.Empty;
            }
        }

        internal void Refresh(CultureInfo culture)
        {
            _culture = culture;
            OnPropertyChanged(nameof(Text));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
