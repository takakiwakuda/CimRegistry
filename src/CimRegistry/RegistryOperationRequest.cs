// (c) Takaki Wakuda.

using Microsoft.Management.Infrastructure;
using Microsoft.Management.Infrastructure.Options;
using Microsoft.Win32;

namespace CimRegistry;

internal enum CimProviderArchitecture
{
    Default = 0,
    Use32Bit = 32,
    Use64Bit = 64,
}

public class RegistryOperationRequest
{
    private const string DefaultSubKeyName = "";

    public required RegistryHive Hive
    {
        get => _hKey;
        set
        {
            ThrowHelper.ThrowIfInvalidEnum(value, nameof(Hive));
            _hKey = value;
        }
    }

    public string SubKeyName
    {
        get => _subKeyName ?? DefaultSubKeyName;
        set
        {
            ThrowHelper.ThrowIfNull(value, nameof(SubKeyName));
            _subKeyName = value;
        }
    }

    public RegistryView View
    {
        get => _view;
        set
        {
            ThrowHelper.ThrowIfInvalidEnum(value, nameof(View));
            _view = value;
        }
    }

    internal CimProviderArchitecture ProviderArchitecture => _view switch
    {
        RegistryView.Registry64 => CimProviderArchitecture.Use64Bit,
        RegistryView.Registry32 => CimProviderArchitecture.Use32Bit,
        _ => CimProviderArchitecture.Default,
    };

    private RegistryHive _hKey;
    private string? _subKeyName;
    private RegistryView _view;

    internal CimOperationOptions GetCimOperationOptions()
    {
        var options = new CimOperationOptions();
        if (_view != RegistryView.Default)
        {
            options.SetCustomOption(CustomOptions.ProviderArchitecture, ProviderArchitecture, CimType.SInt32, true);
            options.SetCustomOption(CustomOptions.RequiredArchitecture, true, CimType.Boolean, true);
        }
        return options;
    }
}

public class RegistryGetValueRequest : RegistryOperationRequest
{
    public string? ValueName
    {
        get => _valueName;
        set => _valueName = value;
    }

    private string? _valueName;
}
