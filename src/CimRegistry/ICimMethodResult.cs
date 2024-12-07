// (c) Takaki Wakuda.

using Microsoft.Management.Infrastructure;
using Microsoft.Management.Infrastructure.Generic;

namespace CimRegistry;

internal interface ICimMethodResult : IDisposable
{
    CimReadOnlyKeyedCollection<CimMethodParameter> OutParameters { get; }

    CimMethodParameter ReturnValue { get; }

    /// <summary>
    /// Gets the value of the <see cref="ReturnValue"/> as a signed 32-bit integer.
    /// </summary>
    /// <remarks>
    /// The <see cref="ReturnCode"/> is not present in the <see cref="CimMethodResult"/> class.
    /// </remarks>
    int ReturnCode { get; }
}

internal sealed class CimMethodResultAdapter(CimMethodResult method) : ICimMethodResult
{
    public CimReadOnlyKeyedCollection<CimMethodParameter> OutParameters => _method.OutParameters;

    public CimMethodParameter ReturnValue => _method.ReturnValue;

    /// <inheritdoc/>
    public int ReturnCode => (int)(uint)ReturnValue.Value;

    private readonly CimMethodResult _method = method;
    private bool _disposed;

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _disposed = true;
        _method.Dispose();
    }
}
