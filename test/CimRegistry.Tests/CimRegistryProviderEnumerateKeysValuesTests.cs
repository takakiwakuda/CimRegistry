using Microsoft.Win32;

namespace CimRegistry.Tests;

public class CimRegistryProviderEnumerateKeysValuesTests : CimRegistryProviderEnumerateKeysValuesTestBase
{
    [Fact]
    public void EnumerateKeys_NullRequest_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("request", () => registryProvider.EnumerateKeys(null));
    }

    [Fact]
    public void EnumerateKeys_AfterDisposed_ThrowsObjectDisposedException()
    {
        Dispose();

        var exception = Assert.Throws<ObjectDisposedException>(() => registryProvider.EnumerateKeys(request));
        Assert.Equal(typeof(CimRegistryProvider).FullName, exception.ObjectName);
    }

    [Fact]
    public void EnumerateKeys_ReturnsExpectedValue()
    {
        string[]? expected = ["key1", "key2", "key3", "key4"];
        SetupEnumerateKeys(SystemErrors.ERROR_SUCCESS, expected);

        var response = registryProvider.EnumerateKeys(request);

        Assert.Equal(SystemErrors.ERROR_SUCCESS, response.ReturnCode);
        Assert.Equal(expected, response.Keys);
        Assert.True(response.IsSuccess);
    }

    [Fact]
    public void EnumerateKeys_NoSubKeys_ReturnsEmptyKeys()
    {
        SetupEnumerateKeys(SystemErrors.ERROR_SUCCESS, null);

        var response = registryProvider.EnumerateKeys(request);

        Assert.Equal(SystemErrors.ERROR_SUCCESS, response.ReturnCode);
        Assert.Empty(response.Keys);
        Assert.True(response.IsSuccess);
    }

    [Fact]
    public void EnumerateKeys_RegistryKeyNotFound_ReturnsFailure()
    {
        SetupEnumerateKeys(SystemErrors.ERROR_FILE_NOT_FOUND, null);

        var response = registryProvider.EnumerateKeys(request);

        Assert.Equal(SystemErrors.ERROR_FILE_NOT_FOUND, response.ReturnCode);
        Assert.Empty(response.Keys);
        Assert.False(response.IsSuccess);
    }

    [Fact]
    public void EnumerateValues_ReturnsExpectedValue()
    {
        string[]? names = ["key1", "key2", "key3", "key4"];
        int[]? valueKinds = [1, 2, 3, 4];
        var expected = names.Select((name, index) => new RegistryValueInfo(name, (RegistryValueKind)valueKinds[index]))
                            .ToArray();
        SetupEnumerateValues(SystemErrors.ERROR_SUCCESS, names, valueKinds);

        var response = registryProvider.EnumerateValues(request);

        Assert.Equal(SystemErrors.ERROR_SUCCESS, response.ReturnCode);
        Assert.Equal(expected, response.Values);
        Assert.True(response.IsSuccess);
    }

    [Fact]
    public void EnumerateValues_NoValues_ReturnsEmptyValues()
    {
        SetupEnumerateValues(SystemErrors.ERROR_SUCCESS, null, null);

        var response = registryProvider.EnumerateValues(request);

        Assert.Equal(SystemErrors.ERROR_SUCCESS, response.ReturnCode);
        Assert.Empty(response.Values);
        Assert.True(response.IsSuccess);
    }

    [Fact]
    public void EnumerateValues_RegistryKeyNotFound_ReturnsFailure()
    {
        SetupEnumerateValues(SystemErrors.ERROR_FILE_NOT_FOUND, null, null);

        var response = registryProvider.EnumerateValues(request);

        Assert.Equal(SystemErrors.ERROR_FILE_NOT_FOUND, response.ReturnCode);
        Assert.Empty(response.Values);
        Assert.False(response.IsSuccess);
    }
}
