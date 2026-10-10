using Xunit;

namespace LiveResx.Avalonia.E2ETests;

/// <summary>
/// Test classes that switch the locale of the <see cref="DynamicLocalization"/> singleton
/// must share this collection so xUnit does not run them in parallel.
/// </summary>
[CollectionDefinition(Name)]
public sealed class DynamicLocalizationCollection
{
    public const string Name = "DynamicLocalization";
}
