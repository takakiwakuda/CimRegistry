// (c) Takaki Wakuda.

using Microsoft.Management.Infrastructure;
using Microsoft.Management.Infrastructure.Options;

namespace CimRegistry;

internal interface ICimSession : IDisposable
{
    string ComputerName { get; }

    ICimMethodResult InvokeMethod(
        string namespaceName,
        string className,
        string methodName,
        CimMethodParametersCollection methodParameters,
        CimOperationOptions options);
}

internal sealed class CimSessionAdapter(CimSession session) : ICimSession
{
    public string ComputerName => _session.ComputerName;

    private readonly CimSession _session = session;
    private bool _disposed;

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _disposed = true;
        _session.Dispose();
    }

    public ICimMethodResult InvokeMethod(
        string namespaceName,
        string className,
        string methodName,
        CimMethodParametersCollection methodParameters,
        CimOperationOptions options)
    {
        var result = _session.InvokeMethod(namespaceName, className, methodName, methodParameters, options);
        return new CimMethodResultAdapter(result);
    }
}
