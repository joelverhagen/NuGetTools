param (
    [string] $PackageDir,
    [switch] $Force
)

Write-Host "Downloading NuGet packages for the website..."

$PackageDir = Resolve-Path $PackageDir

if ($Force -and (Test-Path $PackageDir)) {
    Write-Host "Deleting existing directory..."
    Remove-Item -Force -Recurse $PackageDir
}

$toolDir = Join-Path $PSScriptRoot "src/Knapcode.NuGetTools.PackageDownloader"
& dotnet run --project $toolDir --configuration Release -- download $PackageDir
if ($LASTEXITCODE -ne 0) {
    throw "Package downloader failed with exit code $LastExitCode."  
}

function Remove-ExtraFiles($pattern) {
    Write-Host "Deleting $pattern files"
    Get-ChildItem (Join-Path $PackageDir $pattern) -Recurse | Remove-Item
}

# leave DLLs for loading at runtime and .sha512 for existence check
Remove-ExtraFiles "*.nupkg"
Remove-ExtraFiles "*.nuspec"
Remove-ExtraFiles "*.xml"
Remove-ExtraFiles "*.png"
Remove-ExtraFiles "*.md"
Remove-ExtraFiles ".signature.p7s"
Remove-ExtraFiles ".nupkg.metadata"
