using System.ComponentModel;
using Microsoft.Win32;

namespace CimRegistry.Tests;

public class RegistryOperationRequestTests
{
    [Fact]
    public void Constructor_InvalidRegistryHive_ThrowsInvalidEnumArgumentException()
    {
        Assert.Throws<InvalidEnumArgumentException>("Hive", () => new RegistryOperationRequest() { Hive = 0 });
    }

    [Fact]
    public void Constructor_NullSubKeyName_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("SubKeyName", () => new RegistryOperationRequest()
        {
            Hive = RegistryHive.CurrentUser,
            SubKeyName = null
        });
    }

    [Fact]
    public void Constructor_InvalidRegistryView_ThrowsInvalidEnumArgumentException()
    {
        Assert.Throws<InvalidEnumArgumentException>("View", () => new RegistryOperationRequest()
        {
            Hive = RegistryHive.CurrentUser,
            View = (RegistryView)1
        });
    }

    [Fact]
    public void Properties_WithSpecifiedValues_ReturnsExpectedValue()
    {
        var request = new RegistryOperationRequest()
        {
            Hive = RegistryHive.CurrentUser,
            SubKeyName = "subkey",
            View = RegistryView.Registry64,
        };

        Assert.Equal(RegistryHive.CurrentUser, request.Hive);
        Assert.Equal("subkey", request.SubKeyName);
        Assert.Equal(RegistryView.Registry64, request.View);
    }

    [Fact]
    public void Properties_NotSpecified_ReturnsExpectedValue()
    {
        var request = new RegistryOperationRequest() { Hive = RegistryHive.CurrentUser };

        Assert.Equal(RegistryHive.CurrentUser, request.Hive);
        Assert.Empty(request.SubKeyName);
        Assert.Equal(RegistryView.Default, request.View);
    }

    [Theory]
    [InlineData(RegistryView.Default, 0)]
    [InlineData(RegistryView.Registry32, 32)]
    [InlineData(RegistryView.Registry64, 64)]
    public void ProviderArchitecture_RegistryViewConversion_ReturnsExpectedValue(RegistryView view, int expected)
    {
        var request = new RegistryOperationRequest()
        {
            Hive = RegistryHive.CurrentUser,
            View = view,
        };

        Assert.Equal((CimProviderArchitecture)expected, request.ProviderArchitecture);
    }
}
