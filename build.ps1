param (
    [switch] $SkipPrepare,
    [switch] $SkipRestore,
    [switch] $SkipBuild,
    [switch] $SkipPackageDownload,
    [switch] $SkipTests,
    [switch] $SkipPublish,
    [switch] $IsAppVeyor
)

$root = $PSScriptRoot
$solution = Join-Path $root "NuGetTools.sln"
$lastTraceTime = Get-Date

function Trace-Time() {
    $currentTime = Get-Date
    $lastTime = $lastTraceTime
    $lastTraceTime = $currentTime
    "{0:HH:mm:ss} +{1:F0}" -f $currentTime, ($currentTime - $lastTime).TotalSeconds
}

function Trace-Information($TraceMessage = '') {
    Write-Host "[$(Trace-Time)]`t$TraceMessage" -ForegroundColor Cyan
}

function Get-ProjectDir
{
    param([string] $Dir, [string] $Name)
    ([IO.Path]::Combine($root, $dir, $name))
}

function Get-Project
{
    param([string] $Dir, [string] $Name)
    Join-Path (Get-ProjectDir -Dir $Dir -Name $Name) "$Name.csproj"
}

function Get-BuildProject
{
    param([string] $Name)
    Get-Project -Dir "build" -Name $Name
}

function Get-SrcProject
{
    param([string] $Name)
    Get-Project -Dir "src" -Name $Name
}

function Get-SrcDir
{
    param([string] $Name)
    Get-ProjectDir -Dir "src" -Name $Name
}

function Show-ErrorExitCode
{
    param ([int[]] $SuccessCodes = @(0))
    if ($SuccessCodes -NotContains $LastExitCode)
    {
        throw "Exit code $LastExitCode."
    }
}

if (-Not $SkipPrepare) {
    Trace-Information "Preparing build environment..."
    if ($IsAppVeyor) {
        & npm cache clean
        & npm install -g "bower@1.8.0" "gulp@3.9.1"
    }

    $dotnetCliDir = Join-Path $root "cli"
    $dotnet = Join-Path $dotnetCliDir "dotnet.exe"

    New-Item $dotnetCliDir -Force -Type Directory | Out-Null
    $dotnetCliUrl = "https://raw.githubusercontent.com/dotnet/cli/90f7007f5ef54867b6d697d1320acf92c3850c45/scripts/obtain/dotnet-install.ps1"
    $dotnetCliInstallScript = Join-Path $dotnetCliDir "dotnet-install.ps1"
    Invoke-WebRequest $dotnetCliUrl -OutFile $dotnetCliInstallScript

    & $dotnetCliInstallScript -InstallDir $dotnetCliDir -Version 2.2.402
    Show-ErrorExitCode
} else {
    Trace-Information "Skipped prepare."
}

if (-Not $SkipRestore) {
    Trace-Information "Restoring projects..."
    & $dotnet restore $solution
    Show-ErrorExitCode
} else {
    Trace-Information "Skipped restore."
}

if (-Not $SkipBuild) {
    Trace-Information "Generating assembly info..."
    $versionSuffix = @()
    if ($IsAppVeyor) {
        $buildNumber = $env:APPVEYOR_BUILD_NUMBER
        $versionSuffix = @("--version-suffix", "-$buildNumber")
    }

    $assemblyInfo = & $dotnet run --project (Get-BuildProject -Name "Knapcode.NuGetTools.Build") -- assembly-info --base-directory $root $versionSuffix
    Show-ErrorExitCode

    if ($IsAppVeyor) {
        Trace-Information "Setting AppVeyor build version..."
        $match = [regex]::Match($assemblyInfo, 'InformationalVersion:\s*([^\s]+)')
        if (-Not $match.Success) {
            throw "No version found in the assembly info output."
        }

        $version = $match.Groups[1].Value
        Update-AppveyorBuild -Version $version
    }
    
    Trace-Information "Building..."
    & $dotnet build $solution
    Show-ErrorExitCode
} else {
    Trace-Information "Skipped build."
}

if (-Not $SkipPackageDownload) {
    Trace-Information "Downloading NuGet packages for the website..."
    $packagesDir = Join-Path (Get-SrcDir -Name "Knapcode.NuGetTools.Website") "packages"
    if (Test-Path $packagesDir) {
        Remove-Item -Force -Recurse $packagesDir
    }
    & $dotnet run --project (Get-SrcProject -Name "Knapcode.NuGetTools.PackageDownloader") -- $packagesDir
    Get-ChildItem (Join-Path $packagesDir "*.nupkg") -Recurse | Remove-Item
    Get-ChildItem (Join-Path $packagesDir "*.nuspec") -Recurse | Remove-Item
    Get-ChildItem (Join-Path $packagesDir "*.xml") -Recurse | Remove-Item
    Get-ChildItem (Join-Path $packagesDir ".signature.p7s") -Recurse | Remove-Item
    Get-ChildItem (Join-Path $packagesDir ".nupkg.metadata") -Recurse | Remove-Item
    Show-ErrorExitCode
} else {
    Trace-Information "Skipped package download."
}

if (-Not $SkipTests) {
    Trace-Information "Testing..."
    $projectsToTest = Get-ChildItem (Join-Path $root "test") -Recurse -Include "*.csproj"
    foreach ($projectToTest in $projectsToTest)
    {
        & $dotnet test $projectToTest --no-build --verbosity normal
        Show-ErrorExitCode
    }
} else {
    Trace-Information "Skipped tests."
}

if (-Not $SkipPublish) {
    Trace-Information "Publishing the website..."
    & $dotnet publish (Get-SrcProject -Name "Knapcode.NuGetTools.Website")
    Show-ErrorExitCode
} else {
    Trace-Information "Skipped publish."
}

Trace-Information "Build complete."
