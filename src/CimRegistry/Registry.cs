using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Management.Infrastructure;
using Microsoft.Management.Infrastructure.Options;

namespace CimRegistry;

public sealed class Registry : IDisposable
{
    /// <summary>
    /// Gets the name of the computer opened in the <see cref="Registry"/>.
    /// </summary>
    /// <exception cref="ObjectDisposedException">
    /// The current object has been disposed.
    /// </exception>
    public string ComputerName
    {
        get
        {
            ThrowIfDisposed();
            return _cimSession.ComputerName;
        }
    }

    /// <summary>
    /// Gets the underlying session.
    /// </summary>
    /// <exception cref="ObjectDisposedException">
    /// The current object has been disposed.
    /// </exception>
    public CimSession Session
    {
        get
        {
            ThrowIfDisposed();
            return _cimSession;
        }
    }

    private readonly CimSession _cimSession;
    private readonly bool _leaveOpen;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="Registry"/> class.
    /// </summary>
    /// <exception cref="CimException"/>
    public Registry() : this(Environment.MachineName, new DComSessionOptions())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Registry"/> class
    /// with the specified computer name and optionally session options.
    /// </summary>
    /// <param name="computerName">The computer name to connect to.</param>
    /// <param name="sessionOptions">Session options used when connecting to a computer.</param>
    /// <exception cref="ArgumentException">
    /// <paramref name="computerName"/> is empty.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="computerName"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="CimException"/>
    public Registry(string computerName, CimSessionOptions? sessionOptions = null)
    {
        ThrowHelper.ThrowIfNullOrEmpty(computerName);

        var session = sessionOptions is null ?
            CimSession.Create(computerName) :
            CimSession.Create(computerName, sessionOptions);

        if (!session.TestConnection(out _, out CimException exception))
        {
            session.Dispose();
            throw exception;
        }

        _cimSession = session;
        _leaveOpen = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Registry"/> class
    /// for the specified session and optionally leaves the session open.
    /// </summary>
    /// <param name="session">The session to use.</param>
    /// <param name="leaveOpen">
    /// <see langword="true"/> to leave the session open after disposing the <see cref="Registry"/> object;
    /// otherwise, <see langword="false"/>.
    /// </param>
    /// <exception cref="CimException"/>
    public Registry(CimSession session, bool leaveOpen = false)
    {
        if (!session.TestConnection(out _, out CimException exception))
        {
            throw exception;
        }

        _cimSession = session;
        _leaveOpen = leaveOpen;
    }

    /// <summary>
    /// Releases all resources used by the <see cref="Registry"/>.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _disposed = true;

        if (!_leaveOpen)
        {
            _cimSession.Dispose();
        }
    }

    /// <summary>
    /// Gets the 32-bit unsigned integer associated the specified key and name.
    /// </summary>
    /// <param name="hKey">The HKEY to use.</param>
    /// <param name="keyName">The name of the subkey to use.</param>
    /// <param name="valueName">The name of the value to retrieve.</param>
    /// <param name="view">The registry view to use.</param>
    /// <returns>
    /// The 32-bit unsigned integer associated with <paramref name="valueName"/>,
    /// or <see langword="null"/> if <paramref name="keyName"/> or <paramref name="valueName"/> is not found.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// <paramref name="keyName"/> is empty.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="keyName"/> or <paramref name="valueName"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidEnumArgumentException">
    /// <paramref name="hKey"/> is not the <see cref="RegistryHive"/> enumeration members.
    /// or
    /// <paramref name="view"/> is not the <see cref="RegistryView"/> enumeration members.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// The current object has been disposed.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException"/>
    public uint? GetDWordValue(
        RegistryHive hKey,
        string keyName,
        string valueName,
        RegistryView view = RegistryView.Default)
    {
        ThrowIfDisposed();
        ThrowHelper.ThrowIfInvalidEnum<RegistryHive>(hKey);
        ThrowHelper.ThrowIfInvalidEnum<RegistryView>(view);
        ThrowHelper.ThrowIfNullOrEmpty(keyName);
        ThrowHelper.ThrowIfNull(valueName);

        using var result = GetValue(hKey, keyName, valueName, view);
        if ((uint)result.ReturnValue.Value == Errors.ERROR_ACCESS_DENIED)
        {
            throw new UnauthorizedAccessException();
        }

        return (uint?)result.OutParameters["uValue"].Value;
    }

    /// <summary>
    /// Gets the string associated the specified key and name.
    /// </summary>
    /// <param name="hKey">The HKEY to use.</param>
    /// <param name="keyName">The name of the subkey to use.</param>
    /// <param name="valueName">The name of the value to retrieve.</param>
    /// <param name="view">The registry view to use.</param>
    /// <returns>
    /// The string associated with <paramref name="valueName"/>,
    /// or <see langword="null"/> if <paramref name="keyName"/> or <paramref name="valueName"/> is not found.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// <paramref name="keyName"/> is empty.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="keyName"/> or <paramref name="valueName"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidEnumArgumentException">
    /// <paramref name="hKey"/> is not the <see cref="RegistryHive"/> enumeration members.
    /// or
    /// <paramref name="view"/> is not the <see cref="RegistryView"/> enumeration members.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// The current object has been disposed.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException"/>
    public string? GetStringValue(
        RegistryHive hKey,
        string keyName,
        string valueName,
        RegistryView view = RegistryView.Default)
    {
        ThrowIfDisposed();
        ThrowHelper.ThrowIfInvalidEnum<RegistryHive>(hKey);
        ThrowHelper.ThrowIfInvalidEnum<RegistryView>(view);
        ThrowHelper.ThrowIfNullOrEmpty(keyName);
        ThrowHelper.ThrowIfNull(valueName);

        using var result = GetValue(hKey, keyName, valueName, view);
        if ((uint)result.ReturnValue.Value == Errors.ERROR_ACCESS_DENIED)
        {
            throw new UnauthorizedAccessException();
        }

        return (string?)result.OutParameters["sValue"].Value;
    }

    /// <summary>
    /// Gets the value associated the specified key and name.
    /// </summary>
    /// <param name="hKey">The HKEY to use.</param>
    /// <param name="keyName">The name of the subkey to use.</param>
    /// <param name="valueName">The name of the value to retrieve.</param>
    /// <param name="view">The registry view to use.</param>
    /// <param name="methodName">The method name of the StdRegProv class to retrieve the value.</param>
    /// <returns>
    /// The value associated with <paramref name="valueName"/>,
    /// or <see langword="null"/> if <paramref name="keyName"/> or <paramref name="valueName"/> is not found.
    /// </returns>
    private CimMethodResult GetValue(
        RegistryHive hKey,
        string keyName,
        string valueName,
        RegistryView view,
        [CallerMemberName] string? methodName = null)
    {
        using CimMethodParametersCollection methodParameters = new()
        {
            CimMethodParameter.Create("hDefKey", hKey, CimType.UInt32, CimFlags.In),
            CimMethodParameter.Create("sSubKeyName", keyName, CimType.String, CimFlags.In),
            CimMethodParameter.Create("sValueName", valueName, CimType.String, CimFlags.In)
        };

        using CimOperationOptions options = new();
        if (view != RegistryView.Default)
        {
            options.SetCustomOption("__ProviderArchitecture", (int)view, true);
            options.SetCustomOption("__RequiredArchitecture", true, true);
        }

        return _cimSession.InvokeMethod("root\\default", "StdRegProv", methodName, methodParameters, options);
    }

    /// <summary>
    /// Throws an <see cref="ObjectDisposedException"/> if the current object has already been disposed.
    /// </summary>
    /// <exception cref="ObjectDisposedException">
    /// The current object has been disposed.
    /// </exception>
    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(Registry));
        }
    }
}
