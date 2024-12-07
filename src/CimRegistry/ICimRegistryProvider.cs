// (c) Takaki Wakuda.

using Microsoft.Win32;

namespace CimRegistry;

public interface ICimRegistryProvider : IDisposable
{
    string ComputerName { get; }

    RegistryKeyEnumerationResponse EnumerateKeys(RegistryOperationRequest request);

    RegistryValueEnumerationResponse EnumerateValues(RegistryOperationRequest request);

    RegistryGetValueResponse<byte[]?> GetBinaryValue(RegistryGetValueRequest request);

    RegistryGetValueResponse<uint?> GetDWordValue(RegistryGetValueRequest request);

    RegistryGetValueResponse<string?> GetExpandedStringValue(RegistryGetValueRequest request);

    RegistryGetValueResponse<string[]?> GetMultiStringValue(RegistryGetValueRequest request);

    RegistryGetValueResponse<ulong?> GetQWordValue(RegistryGetValueRequest request);

    RegistryGetValueResponse<string?> GetStringValue(RegistryGetValueRequest request);
}
