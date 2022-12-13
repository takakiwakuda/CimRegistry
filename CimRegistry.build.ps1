[CmdletBinding()]
param (
    [Parameter()]
    [ValidateSet("Debug", "Release")]
    [string]
    $Configuration = (property Configuration Release),

    [Parameter()]
    [ValidateSet("net462", "net7.0")]
    [string]
    $Framework
)

if ($Framework.Length -eq 0) {
    $Framework = if ($PSEdition -eq "Core") { "net7.0" } else { "net462" }
}

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

<#
.SYNOPSIS
    Build CimRegistry.Tests assembly
#>
task BuildCimRegistryTests @{
    Inputs  = {
        Get-ChildItem tests/CimRegistry.Tests/*.cs, tests/CimRegistry.Tests/CimRegistry.Tests.csproj
    }
    Outputs = "tests/CimRegistry.Tests/bin/$Configuration/$Framework/CimRegistry.Tests.dll"
    Jobs    = {
        exec { dotnet build -c $Configuration -f $Framework tests/CimRegistry.Tests }
    }
}

<#
.SYNOPSIS
    Run CimRegistry tests
#>
task RunCimRegistryTests BuildCimRegistryTests, {
    exec { dotnet test --no-build -c $Configuration -f $Framework tests/CimRegistry.Tests }
}

task . RunCimRegistryTests
