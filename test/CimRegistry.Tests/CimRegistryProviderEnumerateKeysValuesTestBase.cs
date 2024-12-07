using Microsoft.Management.Infrastructure;

namespace CimRegistry.Tests;

public abstract class CimRegistryProviderEnumerateKeysValuesTestBase : CimRegistryProviderTestBase
{
    protected readonly RegistryOperationRequest request;

    public CimRegistryProviderEnumerateKeysValuesTestBase() : base()
    {
        request = new RegistryOperationRequest()
        {
            Hive = hKey,
            SubKeyName = subKeyName,
        };
    }

    public void SetupEnumerateKeys(uint returnValue, string[]? names)
    {
        var flags = CimFlagsHelper.Determine(returnValue, names);
        var sNames = CimMethodParameter.Create(MethodParameters.sNames, names, CimType.StringArray, flags);
        Setup(RegistryMethods.EnumKey, returnValue, sNames);
    }

    public void SetupEnumerateValues(uint returnValue, string[]? names, int[]? valueKinds)
    {
        var flags = CimFlagsHelper.Determine(returnValue, names);
        var sNames = CimMethodParameter.Create(MethodParameters.sNames, names, CimType.StringArray, flags);
        var types = CimMethodParameter.Create(MethodParameters.Types, valueKinds, CimType.SInt32Array, flags);
        Setup(RegistryMethods.EnumValues, returnValue, sNames, types);
    }
}
