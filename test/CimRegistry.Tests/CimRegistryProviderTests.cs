using Microsoft.Management.Infrastructure;
using Microsoft.Management.Infrastructure.Options;

namespace CimRegistry.Tests;

public class CimRegistryProviderTests
{
    [Fact]
    public void Constructor_NullSession_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("session", () => new CimRegistryProvider((CimSession)null));
    }

    [Fact]
    public void ComputerName_NoArguments_ReturnsDefaultName()
    {
        using var registryProvider = new CimRegistryProvider();

        Assert.Equal(CimRegistryProvider.DefaultComputerName, registryProvider.ComputerName);
    }

    [Fact]
    public void ComputerName_WithCimSession_ReturnsSpecifiedName()
    {
        using var session = CreateCimSession(Environment.MachineName);
        using var registryProvider = new CimRegistryProvider(session);

        Assert.Equal(Environment.MachineName, registryProvider.ComputerName);
    }

    [Fact]
    public void Dispose_CimSessionAlsoDisposed_ThrowsObjectDisposedException()
    {
        var session = CreateCimSession(CimRegistryProvider.DefaultComputerName);
        var registryProvider = new CimRegistryProvider(session);

        registryProvider.Dispose();

        Assert.Throws<ObjectDisposedException>(() => session.TestConnection());
    }

    [Fact]
    public void Dispose_CimSessionLeftOpen_AllowsFurtherOperations()
    {
        using var session = CreateCimSession(CimRegistryProvider.DefaultComputerName);
        var registryProvider = new CimRegistryProvider(session, leaveOpen: true);

        registryProvider.Dispose();

        Assert.True(session.TestConnection());
    }

    private static CimSession CreateCimSession(string computerName)
    {
        return CimSession.Create(computerName, new DComSessionOptions());
    }
}
