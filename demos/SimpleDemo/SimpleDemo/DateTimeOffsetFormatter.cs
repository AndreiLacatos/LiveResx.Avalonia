using System.Globalization;
using Avalonia.Data.Converters;

namespace SimpleDemo;

public class DateTimeOffsetFormatter : IValueConverter
{
    public string Format { get; set; } = "yyyy-MM-dd HH:mm";

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTimeOffset dto)
            return dto.ToString(parameter as string ?? Format, culture);

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}