namespace CimRegistry.Tests;

public class CimRegistryProviderGetValueTests : CimRegistryProviderGetValueTestBase
{
    [Fact]
    public void GetDWordValue_AfterDisposed_ThrowsObjectDisposedException()
    {
        Dispose();

        var exception = Assert.Throws<ObjectDisposedException>(() => registryProvider.GetDWordValue(request));
        Assert.Equal(typeof(CimRegistryProvider).FullName, exception.ObjectName);
    }

    [Fact]
    public void GetDWordValue_ReturnsExpectedValue()
    {
        uint? expected = 1;
        SetupGetDwordValue(SystemErrors.ERROR_SUCCESS, expected);

        var response = registryProvider.GetDWordValue(request);

        AssertSuccess(expected, response);
    }

    [Fact]
    public void GetDWordValue_RegistryKeyNotFound_ReturnsFailure()
    {
        SetupGetDwordValue(SystemErrors.ERROR_FILE_NOT_FOUND, null);

        var response = registryProvider.GetDWordValue(request);

        AssertFailure(SystemErrors.ERROR_FILE_NOT_FOUND, response);
    }

    [Fact]
    public void GetBinaryValue_ReturnsExpectedValue()
    {
        byte[]? expected = [1, 2, 3, 4];
        SetupGetBinaryValue(SystemErrors.ERROR_SUCCESS, expected);

        var response = registryProvider.GetBinaryValue(request);

        AssertSuccess(expected, response);
    }

    [Fact]
    public void GetExpandedStringValue_ReturnsExpectedValue()
    {
        string? expected = "abcd";
        SetupGetExpandedStringValue(SystemErrors.ERROR_SUCCESS, expected);

        var response = registryProvider.GetExpandedStringValue(request);

        AssertSuccess(expected, response);
    }

    [Fact]
    public void GetMultiStringValue_ReturnsExpectedValue()
    {
        string[]? expected = ["a", "b", "c", "d"];
        SetupGetMultiStringValue(SystemErrors.ERROR_SUCCESS, expected);

        var response = registryProvider.GetMultiStringValue(request);

        AssertSuccess(expected, response);
    }

    [Fact]
    public void GetQWordValue_ReturnsExpectedValue()
    {
        ulong? expected = 1;
        SetupGetQWordValue(SystemErrors.ERROR_SUCCESS, expected);

        var response = registryProvider.GetQWordValue(request);

        AssertSuccess(expected, response);
    }

    [Fact]
    public void GetStringValue_ReturnsExpectedValue()
    {
        string? expected = "abcd";
        SetupGetStringValue(SystemErrors.ERROR_SUCCESS, expected);

        var response = registryProvider.GetStringValue(request);

        AssertSuccess(expected, response);
    }

    private static void AssertSuccess<T>(T expected, RegistryGetValueResponse<T> response)
    {
        Assert.Equal(SystemErrors.ERROR_SUCCESS, response.ReturnCode);
        Assert.Equal(expected, response.Value);
        Assert.True(response.IsSuccess);
        Assert.True(response.HasValue);
    }

    private static void AssertFailure<T>(int returnCode, RegistryGetValueResponse<T> response)
    {
        Assert.Equal(returnCode, response.ReturnCode);
        Assert.Null(response.Value);
        Assert.False(response.IsSuccess);
        Assert.False(response.HasValue);
    }
}
