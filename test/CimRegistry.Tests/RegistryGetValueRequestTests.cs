using Microsoft.Win32;

namespace CimRegistry.Tests;

public class RegistryGetValueRequestTests
{
    [Fact]
    public void Properties_WithSpecifiedValues_ReturnsExpectedValue()
    {
        var request = new RegistryGetValueRequest()
        {
            Hive = RegistryHive.CurrentUser,
            SubKeyName = "subkey",
            ValueName = "value",
            View = RegistryView.Registry64,
        };

        Assert.Equal(RegistryHive.CurrentUser, request.Hive);
        Assert.Equal("subkey", request.SubKeyName);
        Assert.Equal("value", request.ValueName);
        Assert.Equal(RegistryView.Registry64, request.View);
    }

    [Fact]
    public void Properties_NotSpecified_ReturnsExpectedValue()
    {
        var request = new RegistryGetValueRequest() { Hive = RegistryHive.CurrentUser };

        Assert.Equal(RegistryHive.CurrentUser, request.Hive);
        Assert.Empty(request.SubKeyName);
        Assert.Null(request.ValueName);
        Assert.Equal(RegistryView.Default, request.View);
    }

    [Fact]
    public void Cast_ToRegistryOperationOptions_ReturnsExpectedValue()
    {
        var inherited = new RegistryGetValueRequest()
        {
            Hive = RegistryHive.CurrentUser,
            SubKeyName = "subkey",
            ValueName = "value",
            View = RegistryView.Registry64,
        };

        var request = (RegistryOperationRequest)inherited;

        Assert.Equal(RegistryHive.CurrentUser, request.Hive);
        Assert.Equal("subkey", request.SubKeyName);
        Assert.Equal(RegistryView.Registry64, request.View);
    }
}
