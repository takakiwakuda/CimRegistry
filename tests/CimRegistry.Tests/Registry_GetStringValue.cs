using System;
using System.ComponentModel;
using Xunit;

namespace CimRegistry.Tests;

public class Registry_GetStringValue
{
    [Fact]
    public void ObjectDisposed()
    {
        var registry = Helper.CreateDefaultRegistry();
        registry.Dispose();

        void f() => _ = registry.GetStringValue(RegistryHive.LocalMachine, string.Empty, string.Empty);

        Assert.Throws<ObjectDisposedException>(f);
    }

    [Fact]
    public void RegistryHiveIsInvalid()
    {
        using var registry = Helper.CreateDefaultRegistry();

        void f() => _ = registry.GetStringValue(0, string.Empty, string.Empty);

        Assert.Throws<InvalidEnumArgumentException>(f);
    }

    [Fact]
    public void RegistryViewIsInvalid()
    {
        using var registry = Helper.CreateDefaultRegistry();

        void f() => _ = registry.GetStringValue(RegistryHive.LocalMachine, string.Empty, string.Empty, (RegistryView)1);

        Assert.Throws<InvalidEnumArgumentException>(f);
    }

    [Fact]
    public void AccessDenied()
    {
        using var registry = Helper.CreateDefaultRegistry();

        void f() => _ = registry.GetStringValue(RegistryHive.LocalMachine, @"SAM\SAM", string.Empty);

        Assert.Throws<UnauthorizedAccessException>(f);
    }

    [Theory]
    [InlineData(@"Software\DoesNotExist", "")]
    [InlineData(@"Software\Microsoft\.NETFramework", "DoesNotExist")]
    public void ReturnNull(string keyName, string valueName)
    {
        using var registry = Helper.CreateDefaultRegistry();

        var value = registry.GetStringValue(RegistryHive.LocalMachine, keyName, valueName);

        Assert.Null(value);
    }

    [Theory]
    [InlineData(@"C:\Windows\Microsoft.NET\Framework\", 32)]
    [InlineData(@"C:\Windows\Microsoft.NET\Framework64\", 64)]
    public void ReturnString(string except, int view)
    {
        string keyName = @"Software\Microsoft\.NETFramework";
        string valueName = "InstallRoot";
        using var registry = Helper.CreateDefaultRegistry();

        var value = registry.GetStringValue(RegistryHive.LocalMachine, keyName, valueName, (RegistryView)view);

        Assert.Equal(except, value, true);
    }
}
