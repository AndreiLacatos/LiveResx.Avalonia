using System.Globalization;
using Avalonia.Media;
using LiveResx.Avalonia;

namespace CustomResourcesDemo;

/// <summary>
/// Custom typed resource that provides an accent <see cref="Color"/> per culture.
/// This demonstrates <see cref="ILocalizedResource{T}"/> with a non-string value type.
/// Classes are discovered by the source generator in the same compilation
/// and exposed as typed <c>LocalizedResource&lt;Color&gt;</c> properties on
/// <see cref="DynamicResources"/>.
/// </summary>
public sealed class AccentColor : ILocalizedResource<Color>
{
    public IReadOnlyDictionary<CultureInfo, Color> Values { get; } =
        new Dictionary<CultureInfo, Color>
        {
            { CultureInfo.GetCultureInfo("en"), Color.Parse("#E94560") },
            { CultureInfo.GetCultureInfo("de"), Color.Parse("#FF6B35") },
            { CultureInfo.GetCultureInfo("fr"), Color.Parse("#7C3AED") },
        }.AsReadOnly();
}
