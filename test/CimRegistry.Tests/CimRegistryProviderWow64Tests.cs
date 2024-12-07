using Microsoft.Win32;

namespace CimRegistry.Tests;

/// <summary>
/// These access the actual Windows registry.
/// </summary>
public class CimRegistryProviderWow64Tests : IDisposable
{
    private readonly CimRegistryProvider registryProvider;

    public CimRegistryProviderWow64Tests()
    {
        registryProvider = new CimRegistryProvider();
    }

    public void Dispose()
    {
        registryProvider.Dispose();
    }

    [Fact]
    public void GetDWordValue_Using64BitView_ReturnsExpectedValue()
    {
        var response = registryProvider.GetDWordValue(GetValueRequest(RegistryView.Registry64));

        Assert.Equal(SystemErrors.ERROR_SUCCESS, response.ReturnCode);
        Assert.NotNull(response.Value);
        Assert.True(response.IsSuccess);
        Assert.True(response.HasValue);
    }

    [Fact]
    public void GetDWordValue_Using32BitView_ReturnsFailure()
    {
        var response = registryProvider.GetDWordValue(GetValueRequest(RegistryView.Registry32));

        Assert.Equal(SystemErrors.ERROR_INVALID_FUNCTION, response.ReturnCode);
        Assert.Null(response.Value);
        Assert.False(response.IsSuccess);
        Assert.False(response.HasValue);
    }

    private static RegistryGetValueRequest GetValueRequest(RegistryView view)
    {
        return new RegistryGetValueRequest()
        {
            Hive = RegistryHive.LocalMachine,
            SubKeyName = @"Software\Microsoft\.NETFramework",
            ValueName = "Enable64Bit",
            View = view,
        };
    }
}
