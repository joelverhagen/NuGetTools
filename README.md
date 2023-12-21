# NuGet Tools

A website containing various tools to help understand NuGet.

## Live Website

You can visit NuGet Tools at http://nugettools.azurewebsites.net/.

## Local development

You should be able to open the solution file (`NuGetTools.sln`) in Visual Studio and launch the `Knapcode.NuGetTools.Website` project to start the website. This will allow you to use the web interface for the version of the NuGet client SDK (e.g. NuGet.Frameworks and NuGet.Versioning packages) that is used directly by the project.

If you want to have additional NuGet client versions available, run the `Invoke-DownloadPackages.ps1` script to download all available versions of the NuGet client packages from NuGet.org.

## Supported features

- Parse a NuGet framework
- Parse a NuGet package version
- Parse a NuGet package version range
- Test NuGet framework compatibility
- Compare two NuGet package versions
- Test the "get nearest" NuGet framework algorithm
- Test if a version satisfies a version range
- Test the "get best version match" algorithm 
- Switch between different NuGet versions.

## Future

I'd like to add the following features in the future:

- Determine first available version dynamically -- it's hard coded today
- Interacting with real NuGet packages (either uploaded or from a source)
- Better copy-pasting so you can easily get a snippet to drop in an email or document
- REST API
