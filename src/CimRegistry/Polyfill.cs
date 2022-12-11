#if !NETCOREAPP3_0_OR_GREATER
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    [DebuggerNonUserCode]
    [ExcludeFromCodeCoverage]
    [SuppressMessage("csharp", "IDE0060")]
    internal sealed class CallerArgumentExpressionAttribute : Attribute
    {
        internal CallerArgumentExpressionAttribute(string parameterName)
        {
        }
    }
}

namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, Inherited = false)]
    [DebuggerNonUserCode]
    [ExcludeFromCodeCoverage]
    internal sealed class NotNullAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    [DebuggerNonUserCode]
    [ExcludeFromCodeCoverage]
    internal sealed class DoesNotReturnAttribute : Attribute
    {
    }
}
#endif
