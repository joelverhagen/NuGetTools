param (
    [switch] $SkipRestore,
    [switch] $SkipBuild,
    [switch] $SkipPackageDownload,
    [switch] $SkipTests,
    [switch] $SkipPublish
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

if (-Not $SkipRestore) {
    Trace-Information "Restoring projects..."
    & dotnet restore $solution
    Show-ErrorExitCode
} else {
    Trace-Information "Skipped restore."
}

if (-Not $SkipBuild) {
    Trace-Information "Building..."
    & dotnet build $solution
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
    $versionFile = Join-Path $root "package-versions.txt";
    & dotnet run --project (Get-SrcProject -Name "Knapcode.NuGetTools.PackageDownloader") -- $packagesDir --version-file $versionFile
    Get-ChildItem (Join-Path $packagesDir "*.nupkg") -Recurse | Remove-Item
    Get-ChildItem (Join-Path $packagesDir "*.nuspec") -Recurse | Remove-Item
    Get-ChildItem (Join-Path $packagesDir "*.xml") -Recurse | Remove-Item
    Get-ChildItem (Join-Path $packagesDir "*.png") -Recurse | Remove-Item
    Get-ChildItem (Join-Path $packagesDir "*.md") -Recurse | Remove-Item
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
        & dotnet test $projectToTest --logger "console;verbosity=normal" --no-build
        Show-ErrorExitCode
    }
} else {
    Trace-Information "Skipped tests."
}

if (-Not $SkipPublish) {
    Trace-Information "Publishing the website..."
    & dotnet publish (Get-SrcProject -Name "Knapcode.NuGetTools.Website") --configuration Release
    Show-ErrorExitCode
} else {
    Trace-Information "Skipped publish."
}

Trace-Information "Build complete."
