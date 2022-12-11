[CmdletBinding()]
param (
    [Parameter()]
    [ValidateSet("Debug", "Release")]
    [string]
    $Configuration = (property Configuration Release),

    [Parameter()]
    [ValidateSet("net7.0", "netstandard2.0")]
    [string]
    $Framework = "netstandard2.0"
)

$PSCmdlet.WriteVerbose("Configuration : $Configuration")
$PSCmdlet.WriteVerbose("Framework     : $Framework")

<#
.SYNOPSIS
    Build CimRegistry assembly
#>
task BuildCimRegistry @{
    Inputs  = {
        Get-ChildItem src/CimRegistry/*.cs, src/CimRegistry/CimRegistry.csproj
    }
    Outputs = "src/CimRegistry/bin/$Configuration/$Framework/CimRegistry.dll"
    Jobs    = {
        exec { dotnet build -c $Configuration -f $Framework src/CimRegistry }
    }
}

task . BuildCimRegistry
