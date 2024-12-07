// (c) Takaki Wakuda.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace CimRegistry;

internal static class ThrowHelper
{
    internal static void ThrowIfInvalidEnum<T>(int value, string paramName)
    {
        if (!Enum.IsDefined(typeof(T), value))
        {
            throw new InvalidEnumArgumentException(paramName, value, typeof(T));
        }
    }

    internal static void ThrowIfNull([NotNull] object? value, string paramName)
    {
        if (value is null)
        {
            throw new ArgumentNullException(paramName);
        }
    }
}
