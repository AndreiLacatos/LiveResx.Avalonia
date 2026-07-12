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
        resourceDesignerDetector: Generators.ResourceDesignerDetector.Detect,
        reactiveAssemblyDetector: Generators.ReactiveAssemblyDetector.Detect,
        customResourceDetector: Generators.CustomResourceDetector.Detect);

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
    /// Gets a function that detects whether the compilation references any
    /// reactive assemblies (<c>System.Reactive</c>, <c>ReactiveUI</c>, or
    /// <c>ReactiveUI.Avalonia</c>). In tests, inject a function that returns
    /// a fixed value to control whether the observable extension is emitted.
    /// </summary>
    internal Func<Compilation, CancellationToken, bool> ReactiveAssemblyDetector { get; }

    /// <summary>
    /// Gets a function that detects <see cref="ILocalizedResource{T}"/> implementors
    /// in the compilation. In tests, inject a function that returns known descriptors.
    /// </summary>
    internal Func<Compilation, CancellationToken, IReadOnlyList<LocalizedResourceDescriptor>> CustomResourceDetector { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GeneratorDependencies"/> class.
    /// </summary>
    internal GeneratorDependencies(
        Func<DateTimeOffset> timestampProvider,
        Func<Compilation, CancellationToken, IReadOnlyList<ResourceDesignerType>> resourceDesignerDetector,
        Func<Compilation, CancellationToken, bool> reactiveAssemblyDetector,
        Func<Compilation, CancellationToken, IReadOnlyList<LocalizedResourceDescriptor>> customResourceDetector)
    {
        TimestampProvider = timestampProvider;
        ResourceDesignerDetector = resourceDesignerDetector;
        ReactiveAssemblyDetector = reactiveAssemblyDetector;
        CustomResourceDetector = customResourceDetector;
    }
}
