using System.Collections.Immutable;
using System.Reflection;
using Avalonia.Data;
using LiveResx.Avalonia.SourceGenerators.Generators;
using LiveResx.Avalonia.SourceGenerators.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace LiveResx.Avalonia.SourceGenerators.Tests;

/// <summary>
/// Test harness that creates a <see cref="CSharpCompilation"/> from a source string,
/// runs the <see cref="DynamicTranslationGenerator"/> against it with deterministic
/// dependencies, and returns the result for snapshot verification.
/// </summary>
internal static class SourceGenerationRunner
{
    /// <summary>
    /// Fixed timestamp used in all test runs so generated file headers are deterministic.
    /// </summary>
    internal static readonly DateTimeOffset FixedTimestamp =
        new DateTimeOffset(2024, 1, 15, 10, 0, 0, TimeSpan.Zero);

    /// <summary>
    /// Parses <paramref name="source"/>, creates a compilation referencing the core
    /// <c>LiveResx.Avalonia</c> library and all .NET 9.0 base assemblies, then runs
    /// the source generator with a fixed timestamp and the real
    /// <see cref="ResourceDesignerDetector"/> (or optional overrides).
    /// </summary>
    /// <param name="source">The C# source code to compile, typically containing a
    /// synthetic resource designer type.</param>
    /// <param name="detectorOverride">Optional override for the resource designer
    /// detection function; defaults to <see cref="ResourceDesignerDetector.Detect"/>.</param>
    /// <param name="reactiveDetectorOverride">Optional override for the reactive assembly
    /// detection function; defaults to <c>(_, _) =&gt; false</c> so ToObservable extensions
    /// are not emitted unless explicitly enabled.</param>
    /// <returns>A tuple of the <see cref="GeneratorDriver"/> (for snapshot verification)
    /// and the post-generation compilation diagnostics.</returns>
    internal static (GeneratorDriver driver, ImmutableArray<Diagnostic> diagnostics) Run(
        string source,
        Func<Compilation, CancellationToken, IReadOnlyList<ResourceDesignerType>>? detectorOverride = null,
        Func<Compilation, CancellationToken, bool>? reactiveDetectorOverride = null)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        var references = Basic.Reference.Assemblies.Net90.References.All
            .Append(MetadataReference.CreateFromFile(
                typeof(DynamicTranslation).Assembly.Location))
            .Append(MetadataReference.CreateFromFile(
                typeof(Binding).Assembly.Location))
            .Append(MetadataReference.CreateFromFile(
                Assembly.Load("Avalonia.Base").Location));

        var compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: [syntaxTree],
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var generator = new DynamicTranslationGenerator();
        generator.ConfigureDependencies(new GeneratorDependencies(
            timestampProvider: () => FixedTimestamp,
            resourceDesignerDetector: detectorOverride ?? ResourceDesignerDetector.Detect,
            reactiveAssemblyDetector: reactiveDetectorOverride ?? ((_, _) => false)));

        var driver = CSharpGeneratorDriver.Create(generator)
            .RunGenerators(compilation);

        var afterGen = compilation.AddSyntaxTrees(driver.GetRunResult().GeneratedTrees);
        var diagnostics = afterGen.GetDiagnostics();

        return (driver, diagnostics);
    }
}
