using System.Threading;
using Microsoft.CodeAnalysis;

namespace LiveResx.Avalonia.SourceGenerators.Generators;

/// <summary>
/// Detects whether the compilation references any reactive assemblies
/// (<c>System.Reactive</c>, <c>ReactiveUI</c>, or <c>ReactiveUI.Avalonia</c>).
/// Used by <see cref="ToObservableExtensionGenerator"/> to decide whether to emit
/// the <c>ToObservable()</c> extension method.
/// </summary>
internal static class ReactiveAssemblyDetector
{
    /// <summary>
    /// Walks the compilation's referenced assemblies and returns <c>true</c> if
    /// any of <c>System.Reactive</c>, <c>ReactiveUI</c>, or <c>ReactiveUI.Avalonia</c>
    /// is present.
    /// </summary>
    /// <param name="compilation">The compilation to inspect.</param>
    /// <param name="ct">Cancellation token checked before iteration.</param>
    internal static bool Detect(Compilation compilation, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        foreach (var assembly in compilation.ReferencedAssemblyNames)
        {
            if (assembly.Name is "System.Reactive"
                               or "ReactiveUI"
                               or "ReactiveUI.Avalonia"
                               or "Avalonia.ReactiveUI")
            {
                return true;
            }
        }

        return false;
    }
}
