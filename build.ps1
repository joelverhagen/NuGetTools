# Write out the version.txt file.
(git describe --abbrev=0)     | Out-String | %{ $_.Trim() }  > version.txt
(git describe --long --dirty) | Out-String | %{ $_.Trim() } >> version.txt
(git rev-parse HEAD)          | Out-String | %{ $_.Trim() } >> version.txt

# Build all of the projects.
dotnet build **\project.json --configuration Release
