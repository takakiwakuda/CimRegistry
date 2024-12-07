// (c) Takaki Wakuda.

using System.Diagnostics;
using Microsoft.Management.Infrastructure;
using Microsoft.Management.Infrastructure.Options;
using Microsoft.Win32;

namespace CimRegistry;

/// <summary>
/// Provides access to the Windows registry using the Common Infrastructure Model (CIM).
/// </summary>
public sealed class CimRegistryProvider : ICimRegistryProvider
{
    internal const string DefaultComputerName = "localhost";
    internal const string ClassName = "StdRegProv";
    internal const string Namespace = @"root\default";

    /// <summary>
    /// Gets the name of the computer open for the session.
    /// </summary>
    public string ComputerName => _session.ComputerName;

    private readonly ICimSession _session;
    private readonly bool _closable;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="CimRegistryProvider"/> class.
    /// </summary>
    public CimRegistryProvider() : this(CreateDefaultSession())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CimRegistryProvider"/> class
    /// with the specified <see cref="CimSession"/> and optionally leaves the session open.
    /// </summary>
    /// <param name="session">The session to use.</param>
    /// <param name="leaveOpen">
    /// <see langword="true"/> to leave the session open after the <see cref="CimRegistryProvider"/> is disposed;
    /// otherwise, <see langword="false"/>.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="session"/> is <see langword="null"/>.</exception>
    public CimRegistryProvider(CimSession session, bool leaveOpen = false)
    {
        ThrowHelper.ThrowIfNull(session, nameof(session));

        _session = new CimSessionAdapter(session);
        _closable = !leaveOpen;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CimRegistryProvider"/> class with the <see cref="ICimSession"/>.
    /// </summary>
    internal CimRegistryProvider(ICimSession session)
    {
        _session = session;
    }

    private static CimSession CreateDefaultSession()
    {
        return CimSession.Create(DefaultComputerName, new DComSessionOptions());
    }

    /// <summary>
    /// Releases the resources used by the <see cref="CimRegistryProvider"/>.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _disposed = true;

        if (_closable)
        {
            _session.Dispose();
        }
    }

    public RegistryKeyEnumerationResponse EnumerateKeys(RegistryOperationRequest request)
    {
        using var methodResult = InvokeEnumMethod(RegistryMethods.EnumKey, request);

        var returnCode = methodResult.ReturnCode;
        var sNames = methodResult.OutParameters[MethodParameters.sNames];
        if (returnCode != SystemErrors.ERROR_SUCCESS || sNames.IsNullValue())
        {
            return new RegistryKeyEnumerationResponse(returnCode, []);
        }

        return new RegistryKeyEnumerationResponse(returnCode, (string[])sNames.Value);
    }

    public RegistryValueEnumerationResponse EnumerateValues(RegistryOperationRequest request)
    {
        using var methodResult = InvokeEnumMethod(RegistryMethods.EnumValues, request);

        var returnCode = methodResult.ReturnCode;
        var sNames = methodResult.OutParameters[MethodParameters.sNames];
        if (returnCode != SystemErrors.ERROR_SUCCESS || sNames.IsNullValue())
        {
            return new RegistryValueEnumerationResponse(returnCode, []);
        }

        Debug.Assert(!methodResult.OutParameters[MethodParameters.Types].IsNullValue(), "Types should not be null");

        var names = (string[])sNames.Value;
        var types = (int[])methodResult.OutParameters[MethodParameters.Types].Value;
        var valueKinds = names.Select((name, index) => new RegistryValueInfo(name, (RegistryValueKind)types[index]))
                              .ToArray();

        return new RegistryValueEnumerationResponse(returnCode, valueKinds);
    }

    public RegistryGetValueResponse<byte[]?> GetBinaryValue(RegistryGetValueRequest request)
    {
        return GetValue<byte[]?>(RegistryMethods.GetBinaryValue, MethodParameters.uValue, request);
    }

    public RegistryGetValueResponse<uint?> GetDWordValue(RegistryGetValueRequest request)
    {
        return GetValue<uint?>(RegistryMethods.GetDWordValue, MethodParameters.uValue, request);
    }

    public RegistryGetValueResponse<string?> GetExpandedStringValue(RegistryGetValueRequest request)
    {
        return GetValue<string?>(RegistryMethods.GetExpandedStringValue, MethodParameters.sValue, request);
    }

    public RegistryGetValueResponse<string[]?> GetMultiStringValue(RegistryGetValueRequest request)
    {
        return GetValue<string[]?>(RegistryMethods.GetMultiStringValue, MethodParameters.sValue, request);
    }

    public RegistryGetValueResponse<ulong?> GetQWordValue(RegistryGetValueRequest request)
    {
        return GetValue<ulong?>(RegistryMethods.GetQWordValue, MethodParameters.uValue, request);
    }

    public RegistryGetValueResponse<string?> GetStringValue(RegistryGetValueRequest request)
    {
        return GetValue<string?>(RegistryMethods.GetStringValue, MethodParameters.sValue, request);
    }

    private RegistryGetValueResponse<T> GetValue<T>(
        string methodName,
        string paramName,
        RegistryGetValueRequest request)
    {
        ThrowIfDisposed();

        using var methodParameters = RegistryMethodParameterBuilder.Create(request);
        using var options = request.GetCimOperationOptions();
        using var methodResult = InvokeMethod(methodName, methodParameters, options);
        var returnCode = methodResult.ReturnCode;
        var value = (T)methodResult.OutParameters[paramName].Value;

        return new RegistryGetValueResponse<T>(returnCode, value);
    }

    private ICimMethodResult InvokeEnumMethod(string methodName, RegistryOperationRequest request)
    {
        ThrowIfDisposed();

        using var methodParameters = RegistryMethodParameterBuilder.Create(request);
        using var options = request.GetCimOperationOptions();

        return InvokeMethod(methodName, methodParameters, options);
    }

    private ICimMethodResult InvokeMethod(
        string methodName,
        CimMethodParametersCollection methodParameters,
        CimOperationOptions options)
    {
        return _session.InvokeMethod(Namespace, ClassName, methodName, methodParameters, options);
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(GetType().FullName);
        }
    }
}

file static class CimMethodParameterExtensions
{
    internal static bool IsNullValue(this CimMethodParameter methodParameter)
    {
        return (methodParameter.Flags & CimFlags.NullValue) == CimFlags.NullValue;
    }
}
