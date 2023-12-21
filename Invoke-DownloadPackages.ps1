param (
    [string] $PackagesDir,
    [switch] $Force
)

if (!$PackagesDir) {
    $PackagesDir = Join-Path $PSScriptRoot "src/Knapcode.NuGetTools.Website/packages"
}

$PackagesDir = [IO.Path]::GetFullPath($PackagesDir)

Write-Host "Downloading NuGet packages for the website to $PackagesDir"

if ($Force -and (Test-Path $PackagesDir)) {
    Write-Host "Deleting existing directory"
    Remove-Item -Force -Recurse $PackagesDir
}

$toolDir = Join-Path $PSScriptRoot "src/Knapcode.NuGetTools.PackageDownloader"
& dotnet run --project $toolDir --configuration Release -- download $PackagesDir
if ($LASTEXITCODE -ne 0) {
    throw "Package downloader failed with exit code $LASTEXITCODE."  
}

Write-Host "Successfully downloaded NuGet packages"
