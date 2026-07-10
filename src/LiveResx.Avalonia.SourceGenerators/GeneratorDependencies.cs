using System;

namespace LiveResx.Avalonia.SourceGenerators;

/// <summary>
/// Holds injectable dependencies for the <see cref="DynamicTranslationGenerator"/>.
/// Override via <c>ConfigureDependencies</c> in unit tests to control generator behavior.
/// </summary>
internal sealed class GeneratorDependencies
{
    /// <summary>
    /// Gets the default dependency instance used at build time.
    /// </summary>
    internal static GeneratorDependencies Default { get; } = new GeneratorDependencies(
        timestampProvider: () => DateTimeOffset.UtcNow);

    /// <summary>
    /// Gets a factory for obtaining the current timestamp used in generated file headers.
    /// In tests, inject a fixed-value factory to produce deterministic output.
    /// </summary>
    internal Func<DateTimeOffset> TimestampProvider { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GeneratorDependencies"/> class.
    /// </summary>
    /// <param name="timestampProvider">A factory that provides the timestamp to embed
    /// in generated file headers.</param>
    internal GeneratorDependencies(Func<DateTimeOffset> timestampProvider)
    {
        TimestampProvider = timestampProvider;
    }
}
