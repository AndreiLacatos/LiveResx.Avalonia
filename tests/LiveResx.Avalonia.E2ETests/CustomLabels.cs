using System.Collections.Generic;
using System.Globalization;
using LiveResx.Avalonia;

namespace LiveResx.Avalonia.E2ETests;

internal sealed class CustomLabels : ILocalizedResource<string>
{
    public IReadOnlyDictionary<CultureInfo, string> Values { get; } =
        new Dictionary<CultureInfo, string>
        {
            { new CultureInfo("en"), "English" },
            { new CultureInfo("de"), "Deutsch" },
        }.AsReadOnly();
}
