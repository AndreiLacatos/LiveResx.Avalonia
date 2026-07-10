using System;
using System.Collections.Generic;
using System.Threading;
using LiveResx.Avalonia.SourceGenerators.Generators;
using LiveResx.Avalonia.SourceGenerators.Models;
using Microsoft.CodeAnalysis;

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
        timestampProvider: () => DateTimeOffset.UtcNow,
        resourceDesignerDetector: Generators.ResourceDesignerDetector.Detect);

    /// <summary>
    /// Gets a factory for obtaining the current timestamp used in generated file headers.
    /// In tests, inject a fixed-value factory to produce deterministic output.
    /// </summary>
    internal Func<DateTimeOffset> TimestampProvider { get; }

    /// <summary>
    /// Gets a function that detects resource Designer types in a compilation.
    /// In tests, inject a function that returns known resource types without
    /// requiring actual assemblies or <c>.resx</c> files.
    /// </summary>
    internal Func<Compilation, CancellationToken, IReadOnlyList<ResourceDesignerType>> ResourceDesignerDetector { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GeneratorDependencies"/> class.
    /// </summary>
    internal GeneratorDependencies(
        Func<DateTimeOffset> timestampProvider,
        Func<Compilation, CancellationToken, IReadOnlyList<ResourceDesignerType>> resourceDesignerDetector)
    {
        TimestampProvider = timestampProvider;
        ResourceDesignerDetector = resourceDesignerDetector;
    }
}
