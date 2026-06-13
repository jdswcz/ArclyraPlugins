param(
    [Parameter(Mandatory = $true)]
    [string]$ProjectDir,

    [Parameter(Mandatory = $true)]
    [string]$OutputDir,

    [Parameter(Mandatory = $true)]
    [string]$IntermediateOutputDir,

    [string]$PackageName = "Plugin.arcplugin",

    [string]$ManifestFile = "plugin.json",

    [string]$PluginAssemblyName = "",

    [string[]]$HostSharedAssemblies = @("Arclyra.PluginSdk.dll"),

    [string]$SignToolPath = "",

    [string]$DeveloperName = "",

    [string]$DeveloperPrivateKey = "",

    [string]$DeveloperPublicKey = "",

    [string]$DeveloperSignature = ""
)

$ErrorActionPreference = "Stop"

$projectPath = Resolve-Path -LiteralPath $ProjectDir
$outputPath = Resolve-Path -LiteralPath $OutputDir

if ([System.IO.Path]::IsPathRooted($IntermediateOutputDir)) {
    $intermediateInputPath = $IntermediateOutputDir
}
else {
    $intermediateInputPath = Join-Path $projectPath $IntermediateOutputDir
}

if (-not (Test-Path -LiteralPath $intermediateInputPath)) {
    New-Item -ItemType Directory -Path $intermediateInputPath | Out-Null
}

$intermediatePath = Resolve-Path -LiteralPath $intermediateInputPath
$stagingPath = Join-Path $intermediatePath "arcplugin-package"
$pluginStagingPath = Join-Path $stagingPath "plugin"
$packagePath = Join-Path $outputPath $PackageName
if ([System.IO.Path]::IsPathRooted($ManifestFile)) {
    $pluginManifestPath = $ManifestFile
}
else {
    $pluginManifestPath = Join-Path $projectPath $ManifestFile
}

if (-not (Test-Path -LiteralPath $pluginManifestPath)) {
    throw "Required plugin manifest was not found: $pluginManifestPath"
}

if (Test-Path -LiteralPath $stagingPath) {
    Remove-Item -LiteralPath $stagingPath -Recurse -Force
}

New-Item -ItemType Directory -Path $pluginStagingPath | Out-Null
Copy-Item -LiteralPath $pluginManifestPath -Destination $pluginStagingPath

$hostSharedAssemblyNames = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::OrdinalIgnoreCase)
foreach ($assemblyName in $HostSharedAssemblies) {
    [void]$hostSharedAssemblyNames.Add($assemblyName)
}

Get-ChildItem -LiteralPath $outputPath -Filter "*.dll" -File |
    Where-Object { -not $hostSharedAssemblyNames.Contains($_.Name) } |
    Copy-Item -Destination $pluginStagingPath

if (-not [string]::IsNullOrWhiteSpace($PluginAssemblyName)) {
    $pluginAssemblyPath = Join-Path $pluginStagingPath $PluginAssemblyName
    if (-not (Test-Path -LiteralPath $pluginAssemblyPath)) {
        throw "Required plugin assembly was not staged: $pluginAssemblyPath"
    }
}

if (Test-Path -LiteralPath $packagePath) {
    Remove-Item -LiteralPath $packagePath -Force
}

Add-Type -AssemblyName System.IO.Compression.FileSystem
[System.IO.Compression.ZipFile]::CreateFromDirectory($stagingPath, $packagePath)
if (-not [string]::IsNullOrWhiteSpace($SignToolPath)) {
    if ([string]::IsNullOrWhiteSpace($DeveloperName) -or [string]::IsNullOrWhiteSpace($DeveloperPrivateKey) -or [string]::IsNullOrWhiteSpace($DeveloperPublicKey) -or [string]::IsNullOrWhiteSpace($DeveloperSignature)) {
        throw "Signing requires DeveloperName, DeveloperPrivateKey, DeveloperPublicKey, and DeveloperSignature."
    }

    & $SignToolPath sign-package --package $packagePath --developer-name $DeveloperName --developer-private-key $DeveloperPrivateKey --developer-public-key $DeveloperPublicKey --developer-signature $DeveloperSignature
    if ($LASTEXITCODE -ne 0) {
        throw "Arclyra.PluginSignTool failed with exit code $LASTEXITCODE."
    }
}

Write-Host "Created Arclyra plugin package: $packagePath"
