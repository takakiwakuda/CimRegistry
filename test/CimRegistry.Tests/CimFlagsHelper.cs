using Microsoft.Management.Infrastructure;

namespace CimRegistry.Tests;

public static class CimFlagsHelper
{
    public static CimFlags Determine(uint returnValue, object? value)
    {
        var flags = CimFlags.None;

        if (returnValue == SystemErrors.ERROR_SUCCESS)
        {
            flags |= CimFlags.NotModified;
        }

        if (value is null)
        {
            flags |= CimFlags.NullValue;
        }

        return flags;
    }
}
