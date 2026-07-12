using System.Collections.Generic;
using System.Globalization;

namespace LiveResx.Avalonia
{
    /// <summary>
    /// Defines a source of culture-specific values. Implement this interface on a class
    /// to let the source generator automatically create a <see cref="LocalizedResource{T}"/>,
    /// register it with <see cref="DynamicLocalization"/>, and expose an unwrapped getter
    /// on <see cref="DynamicResources"/>.
    /// </summary>
    /// <typeparam name="T">The type of the resource value.</typeparam>
    public interface ILocalizedResource<T>
    {
        /// <summary>
        /// Gets a read-only dictionary mapping each culture to its corresponding value.
        /// The recommended practice is to provide at least a
        /// <see cref="CultureInfo.InvariantCulture"/> entry as the fallback.
        /// </summary>
        IReadOnlyDictionary<CultureInfo, T> Values { get; }
    }
}
