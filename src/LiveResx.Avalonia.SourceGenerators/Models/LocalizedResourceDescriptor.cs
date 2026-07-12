namespace LiveResx.Avalonia.SourceGenerators.Models;

/// <summary>
/// Represents a user-defined class that implements <see cref="ILocalizedResource{T}"/>,
/// discovered by the <see cref="Generators.CustomResourceDetector"/>.
/// </summary>
internal sealed record LocalizedResourceDescriptor
{
    /// <summary>
    /// Gets the name used for the getter on <see cref="DynamicResources"/>.
    /// Derived from the implementing class name.
    /// </summary>
    public string GetterName { get; }

    /// <summary>
    /// Gets the fully qualified type name of the resource value (<c>T</c>),
    /// including the <c>global::</c> prefix, e.g. <c>"global::System.String"</c>.
    /// </summary>
    public string ValueTypeFullName { get; }

    /// <summary>
    /// Gets the fully qualified name of the implementing class,
    /// including the <c>global::</c> prefix, e.g. <c>"global::MyApp.CountryFlags"</c>.
    /// </summary>
    public string ImplementorFullName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalizedResourceDescriptor"/> class.
    /// </summary>
    /// <param name="getterName">The getter name on DynamicResources (class name).</param>
    /// <param name="valueTypeFullName">Fully qualified value type name with global:: prefix.</param>
    /// <param name="implementorFullName">Fully qualified implementing class name with global:: prefix.</param>
    public LocalizedResourceDescriptor(
        string getterName,
        string valueTypeFullName,
        string implementorFullName)
    {
        GetterName = getterName;
        ValueTypeFullName = valueTypeFullName;
        ImplementorFullName = implementorFullName;
    }
}
