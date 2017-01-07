param (
    [switch] $SkipPrepare,
    [switch] $SkipRestore,
    [switch] $SkipBuild,
    [switch] $SkipPackageDownload,
    [switch] $SkipTests,
    [switch] $SkipPublish
)

$root = $PSScriptRoot
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
    Join-Path (Get-ProjectDir -Dir $Dir -Name $Name) "project.json"
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
    $dotnetCliDir = Join-Path $root "cli"
    $dotnet = Join-Path $dotnetCliDir "dotnet.exe"

    New-Item $dotnetCliDir -Force -Type Directory | Out-Null
    $dotnetCliUrl = "https://raw.githubusercontent.com/dotnet/cli/rel/1.0.0-preview2/scripts/obtain/dotnet-install.ps1"
    $dotnetCliInstallScript = Join-Path $dotnetCliDir "dotnet-install.ps1"
    Invoke-WebRequest $dotnetCliUrl -OutFile $dotnetCliInstallScript

    & $dotnetCliInstallScript -InstallDir $dotnetCliDir -Version 1.0.0-preview2-003156
    Show-ErrorExitCode
} else {
    Trace-Information "Skipped prepare."
}

if (-Not $SkipRestore) {
    Trace-Information "Restoring projects..."
    & $dotnet restore $root
    Show-ErrorExitCode
} else {
    Trace-Information "Skipped restore."
}

if (-Not $SkipBuild) {
    Trace-Information "Generating assembly info..."
    & $dotnet run -p (Get-BuildProject -Name "Knapcode.NuGetTools.Build") -- assemblyInfo --baseDirectory $root
    Show-ErrorExitCode

    Trace-Information "Building..."
    $projectsToBuild = Get-ChildItem $root -Recurse -Include "project.json"
    foreach ($projectToBuild in $projectsToBuild)
    {
       & $dotnet build $projectToBuild
       Show-ErrorExitCode
    }
} else {
    Trace-Information "Skipped build."
}

if (-Not $SkipPackageDownload) {
    Trace-Information "Downloading NuGet packages for the website..."
    & $dotnet run -p (Get-SrcProject -Name "Knapcode.NuGetTools.PackageDownloader") -- (Join-Path (Get-SrcDir -Name "Knapcode.NuGetTools.Website") "packages")
    Show-ErrorExitCode
} else {
    Trace-Information "Skipped package download."
}

if (-Not $SkipTests) {
    Trace-Information "Testing..."
    $projectsToTest = Get-ChildItem (Join-Path $root "test") -Recurse -Include "project.json"
    foreach ($projectToTest in $projectsToTest)
    {
       & $dotnet test $projectToTest
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
