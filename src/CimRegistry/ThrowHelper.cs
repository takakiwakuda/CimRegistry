// (c) Takaki Wakuda.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace CimRegistry;

internal static class ThrowHelper
{
    internal static void ThrowIfInvalidEnum<T>(T value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : Enum
    {
        if (!Enum.IsDefined(typeof(T), value))
        {
            int invalidValue = Convert.ToInt32(value, CultureInfo.InvariantCulture);
            throw new InvalidEnumArgumentException(paramName, invalidValue, typeof(T));
        }
    }

    internal static void ThrowIfNull([NotNull] object? value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (value is null)
        {
            throw new ArgumentNullException(paramName);
        }
    }
}
