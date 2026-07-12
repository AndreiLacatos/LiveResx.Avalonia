using System.Globalization;
using LiveResx.Avalonia;

namespace CustomResourcesDemo;

/// <summary>
/// Custom typed resource that provides a per-culture tagline string.
/// This demonstrates <see cref="ILocalizedResource{T}"/> with a string value type,
/// where the values are defined in code rather than in a <c>.resx</c> file.
/// </summary>
public sealed class AppTagline : ILocalizedResource<string>
{
    public IReadOnlyDictionary<CultureInfo, string> Values { get; } =
        new Dictionary<CultureInfo, string>
        {
            { CultureInfo.GetCultureInfo("en"), "Live-resources, strongly-typed" },
            { CultureInfo.GetCultureInfo("de"), "Live-Ressourcen, stark typisiert" },
            { CultureInfo.GetCultureInfo("fr"), "Ressources dynamiques, fortement typées" },
        }.AsReadOnly();
}
