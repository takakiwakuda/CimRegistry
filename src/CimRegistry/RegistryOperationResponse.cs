// (c) Takaki Wakuda.

using System.Diagnostics.CodeAnalysis;
using Microsoft.Win32;

namespace CimRegistry;

public abstract class RegistryOperationResponse(int returnCode)
{
    public int ReturnCode { get; } = returnCode;

    public bool IsSuccess => ReturnCode == SystemErrors.ERROR_SUCCESS;
}

public class RegistryGetValueResponse<T>(int returnCode, T value) : RegistryOperationResponse(returnCode)
{
    public T Value { get; } = value;

    [MemberNotNullWhen(true, nameof(Value))]
    public bool HasValue => Value is not null;
}

public class RegistryKeyEnumerationResponse : RegistryOperationResponse
{
    public string[] Keys { get; }

    public RegistryKeyEnumerationResponse(int returnCode, string[] keys) : base(returnCode)
    {
        ThrowHelper.ThrowIfNull(keys);

        Keys = keys;
    }
}

public class RegistryValueEnumerationResponse : RegistryOperationResponse
{
    public RegistryValueInfo[] Values { get; }

    public RegistryValueEnumerationResponse(int returnCode, RegistryValueInfo[] values) : base(returnCode)
    {
        ThrowHelper.ThrowIfNull(values);

        Values = values;
    }
}

public record class RegistryValueInfo(string Name, RegistryValueKind ValueKind);
