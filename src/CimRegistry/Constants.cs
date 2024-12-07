// (c) Takaki Wakuda.

namespace CimRegistry;

internal static class CustomOptions
{
    internal const string ProviderArchitecture = "__ProviderArchitecture";
    internal const string RequiredArchitecture = "__RequiredArchitecture";
}

internal static class MethodParameters
{
    internal const string hDefKey = "hDefKey";
    internal const string ReturnValue = "ReturnValue";
    internal const string sNames = "sNames";
    internal const string sSubKeyName = "sSubKeyName";
    internal const string sValueName = "sValueName";
    internal const string sValue = "sValue";
    internal const string Types = "Types";
    internal const string uValue = "uValue";
}

internal static class RegistryMethods
{
    internal const string EnumKey = "EnumKey";
    internal const string EnumValues = "EnumValues";
    internal const string GetBinaryValue = "GetBinaryValue";
    internal const string GetDWordValue = "GetDWORDValue";
    internal const string GetExpandedStringValue = "GetExpandedStringValue";
    internal const string GetMultiStringValue = "GetMultiStringValue";
    internal const string GetQWordValue = "GetQWORDValue";
    internal const string GetStringValue = "GetStringValue";
}

internal static class SystemErrors
{
    internal const int ERROR_SUCCESS = 0x0;
    internal const int ERROR_INVALID_FUNCTION = 0x1;
    internal const int ERROR_FILE_NOT_FOUND = 0x2;
}
