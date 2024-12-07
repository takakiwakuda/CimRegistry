namespace CimRegistry.Tests;

public class RegistryKeyEnumerationResponseTests
{
    [Fact]
    public void Constructor_NullKeys_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("keys", () => new RegistryKeyEnumerationResponse(0, null));
    }
}
