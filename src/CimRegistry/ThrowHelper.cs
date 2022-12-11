using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace CimRegistry;

internal static class ThrowHelper
{
    internal static void ThrowIfInvalidEnum<T>(object argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (!Enum.IsDefined(typeof(T), argument))
        {
            throw new InvalidEnumArgumentException($"The value of argument '{paramName}' ({argument}) is invalid for Enum type '{typeof(T)}'.");
        }
    }

    internal static void ThrowIfNullOrEmpty([NotNull] string? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
#if NET7_0_OR_GREATER
        ArgumentException.ThrowIfNullOrEmpty(argument, paramName);
#else
        ThrowIfNull(argument, paramName);

        if (argument.Length == 0)
        {
            throw new ArgumentException("The value cannot be an empty string.", paramName);
        }
#endif
    }

    internal static void ThrowIfNull([NotNull] object? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(argument, paramName);
#else
        if (argument is null)
        {
            throw new ArgumentNullException(paramName);
        }
#endif
    }
}
