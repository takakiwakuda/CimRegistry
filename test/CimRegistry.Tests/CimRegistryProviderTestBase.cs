using Microsoft.Management.Infrastructure;
using Microsoft.Management.Infrastructure.Options;
using Microsoft.Win32;
using Moq;

namespace CimRegistry.Tests;

public abstract class CimRegistryProviderTestBase : IDisposable
{
    protected const RegistryHive hKey = RegistryHive.CurrentUser;
    protected const string subKeyName = "subkey";

    protected readonly CimRegistryProvider registryProvider;
    private readonly Mock<ICimSession> _session;
    private CimMethodParametersCollection? _methodParameters;

    public CimRegistryProviderTestBase()
    {
        _session = new Mock<ICimSession>();
        registryProvider = new CimRegistryProvider(_session.Object);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        registryProvider.Dispose();
        _methodParameters?.Dispose();
    }

    protected virtual bool IsValidParameters(CimMethodParametersCollection methodParameters)
    {
        return methodParameters[MethodParameters.hDefKey].Value is uint hDefKey
               && hDefKey == unchecked((uint)hKey)
               && methodParameters[MethodParameters.sSubKeyName].Value is string sSubKeyName
               && sSubKeyName == subKeyName;
    }

    protected void Setup(string methodName, uint returnValue, params CimMethodParameter[] newParameters)
    {
        _methodParameters =
        [
            CimMethodParameter.Create(MethodParameters.ReturnValue, returnValue, CimType.UInt32, CimFlags.NotModified)
        ];

        foreach (var newParameter in newParameters)
        {
            _methodParameters.Add(newParameter);
        }

        var methodResult = new Mock<ICimMethodResult>();
        methodResult.SetupGet(r => r.OutParameters).Returns(_methodParameters);
        methodResult.SetupGet(r => r.ReturnValue).Returns(_methodParameters[MethodParameters.ReturnValue]);
        methodResult.SetupGet(r => r.ReturnCode)
                    .Returns((int)(uint)_methodParameters[MethodParameters.ReturnValue].Value);

        _session.Setup(s => s.InvokeMethod(
            CimRegistryProvider.Namespace,
            CimRegistryProvider.ClassName,
            methodName,
            It.Is<CimMethodParametersCollection>(p => IsValidParameters(p)),
            It.IsAny<CimOperationOptions>())).Returns(methodResult.Object);
    }
}
