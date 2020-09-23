# Publish a new package to nuget.org

1. Review and update the file Salience.FluentApi.nuspec
2. Using the tool [nuget.exe](https://dist.nuget.org/win-x86-commandline/latest/nuget.exe), run `nuget pack`
3. Publish the new package Salience.FluentApi.nupkg to nuget.org using either the command `nuget push` or the web portal.

[Documentation](https://docs.microsoft.com/fr-fr/nuget/quickstart/create-and-publish-a-package-using-visual-studio-net-framework)