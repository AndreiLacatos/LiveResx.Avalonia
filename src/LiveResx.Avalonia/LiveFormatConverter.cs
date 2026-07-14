using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;

namespace LiveResx.Avalonia
{
    public sealed class LiveFormatConverter : IMultiValueConverter
    {
        private readonly DynamicTranslation _key;

        public LiveFormatConverter(DynamicTranslation key)
        {
            _key = key;
        }

        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            var args = values.Count > 0
                ? values.Take(values.Count - 1).ToArray()
                : Array.Empty<object>();
            // var template = (string)values[values.Count - 1];
            var template = _key.Text;
            return string.Format(template, args);
        }
    }
}
