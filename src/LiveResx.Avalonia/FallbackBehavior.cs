using System;

namespace LiveResx.Avalonia
{
    /// <summary>
    /// Determines how <see cref="LocalizedResource{T}"/> resolves its <see cref="LocalizedResource{T}.Value"/>
    /// when an exact match for the requested culture is not present in the value dictionary.
    /// </summary>
    public enum FallbackBehavior
    {
        /// <summary>
        /// When an exact culture match is not found, the resource falls back to
        /// the <see cref="CultureInfo.InvariantCulture"/> entry. If that is also missing,
        /// the first available entry is returned. If the dictionary is empty,
        /// <c>default(T)</c> is returned.
        /// </summary>
        Invariant,

        /// <summary>
        /// When an exact culture match is not found, the resource walks the
        /// <see cref="CultureInfo.Parent"/> chain (e.g., <c>"de-DE"</c> → <c>"de"</c>)
        /// until a match is found. If the chain is exhausted without a match,
        /// falls back to the <see cref="CultureInfo.InvariantCulture"/> entry,
        /// then the first available entry, then <c>default(T)</c>.
        /// </summary>
        ParentChain,
    }
}
