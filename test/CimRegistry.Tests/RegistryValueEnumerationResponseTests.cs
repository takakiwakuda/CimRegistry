namespace CimRegistry.Tests;

public class RegistryValueEnumerationResponseTests
{
    [Fact]
    public void Constructor_NullKeys_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("values", () => new RegistryValueEnumerationResponse(0, null));
    }
}
