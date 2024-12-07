using Microsoft.Management.Infrastructure;

namespace CimRegistry.Tests;

public abstract class CimRegistryProviderGetValueTestBase : CimRegistryProviderTestBase
{
    protected const string valueName = "value";

    protected readonly RegistryGetValueRequest request;

    public CimRegistryProviderGetValueTestBase() : base()
    {
        request = new RegistryGetValueRequest()
        {
            Hive = hKey,
            SubKeyName = subKeyName,
            ValueName = valueName,
        };
    }

    public void SetupGetBinaryValue(uint returnValue, byte[]? value)
    {
        var uValue = CreateUValueParameter(returnValue, value, CimType.UInt8Array);
        Setup(RegistryMethods.GetBinaryValue, returnValue, uValue);
    }

    public void SetupGetDwordValue(uint returnValue, uint? value)
    {
        var uValue = CreateUValueParameter(returnValue, value, CimType.UInt32);
        Setup(RegistryMethods.GetDWordValue, returnValue, uValue);
    }

    public void SetupGetExpandedStringValue(uint returnValue, string? value)
    {
        var sValue = CreateSValueParameter(returnValue, value, CimType.String);
        Setup(RegistryMethods.GetExpandedStringValue, returnValue, sValue);
    }

    public void SetupGetMultiStringValue(uint returnValue, string[]? value)
    {
        var sValue = CreateSValueParameter(returnValue, value, CimType.StringArray);
        Setup(RegistryMethods.GetMultiStringValue, returnValue, sValue);
    }

    public void SetupGetQWordValue(uint returnValue, ulong? value)
    {
        var uValue = CreateUValueParameter(returnValue, value, CimType.UInt64);
        Setup(RegistryMethods.GetQWordValue, returnValue, uValue);
    }

    public void SetupGetStringValue(uint returnValue, string? value)
    {
        var sValue = CreateSValueParameter(returnValue, value, CimType.String);
        Setup(RegistryMethods.GetStringValue, returnValue, sValue);
    }

    protected override bool IsValidParameters(CimMethodParametersCollection methodParameters)
    {
        return base.IsValidParameters(methodParameters)
               && methodParameters[MethodParameters.sValueName].Value is string sValueName
               && sValueName == valueName;
    }

    private static CimMethodParameter CreateSValueParameter(uint returnValue, object? value, CimType type)
    {
        var flags = CimFlagsHelper.Determine(returnValue, value);
        return CimMethodParameter.Create(MethodParameters.sValue, value, type, flags);
    }

    private static CimMethodParameter CreateUValueParameter(uint returnValue, object? value, CimType type)
    {
        var flags = CimFlagsHelper.Determine(returnValue, value);
        return CimMethodParameter.Create(MethodParameters.uValue, value, type, flags);
    }
}
