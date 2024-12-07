// (c) Takaki Wakuda.

using Microsoft.Management.Infrastructure;

namespace CimRegistry;

internal static class RegistryMethodParameterBuilder
{
    internal static CimMethodParametersCollection Create(RegistryOperationRequest request)
    {
        return [
            CimMethodParameter.Create(MethodParameters.hDefKey, (uint)request.Hive, CimType.UInt32, CimFlags.In),
            CimMethodParameter.Create(MethodParameters.sSubKeyName, request.SubKeyName, CimType.String, CimFlags.In),
        ];
    }

    internal static CimMethodParametersCollection Create(RegistryGetValueRequest request)
    {
        return [
            CimMethodParameter.Create(MethodParameters.hDefKey, (uint)request.Hive, CimType.UInt32, CimFlags.In),
            CimMethodParameter.Create(MethodParameters.sSubKeyName, request.SubKeyName, CimType.String, CimFlags.In),
            CimMethodParameter.Create(MethodParameters.sValueName, request.ValueName, CimType.String, CimFlags.In),
        ];
    }
}
