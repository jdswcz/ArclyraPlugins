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

    [string]$DeveloperSignature = "",

    [string]$RequireSigning = "false"
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
$mustSign = $RequireSigning -eq "true"
if ($mustSign -and [string]::IsNullOrWhiteSpace($SignToolPath)) {
    if (Test-Path -LiteralPath $packagePath) { Remove-Item -LiteralPath $packagePath -Force }
    throw "This plugin requires ordinary developer signing; SignToolPath cannot be empty."
}
if (-not [string]::IsNullOrWhiteSpace($SignToolPath)) {
    if (Test-Path -LiteralPath $packagePath) { Remove-Item -LiteralPath $packagePath -Force }
    if ([string]::IsNullOrWhiteSpace($DeveloperName)) { throw "DeveloperName is required for signing." }
    foreach ($signingInput in @($SignToolPath, $DeveloperPrivateKey, $DeveloperPublicKey, $DeveloperSignature)) {
        if ([string]::IsNullOrWhiteSpace($signingInput) -or -not (Test-Path -LiteralPath $signingInput -PathType Leaf)) {
            throw "A required signing tool/key/signature file is missing: $signingInput"
        }
    }
}
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
    try {
        & $SignToolPath sign-package --package $packagePath --developer-name $DeveloperName --developer-private-key $DeveloperPrivateKey --developer-public-key $DeveloperPublicKey --developer-signature $DeveloperSignature
        if ($LASTEXITCODE -ne 0) {
            throw "Arclyra.PluginSignTool failed with exit code $LASTEXITCODE."
        }
        if ($mustSign) {
            & $SignToolPath verify-package --package $packagePath --developer-name $DeveloperName
            if ($LASTEXITCODE -ne 0) { throw "Ordinary package verification failed with exit code $LASTEXITCODE." }
        }
    }
    catch {
        if (Test-Path -LiteralPath $packagePath) { Remove-Item -LiteralPath $packagePath -Force }
        throw
    }
}

Write-Host "Created Arclyra plugin package: $packagePath"
