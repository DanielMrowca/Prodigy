$outputLocalNugetPath = "C:\Users\daniel.mrowca\.nuget\localBuildsPackages";
$todayDate = Get-Date -Format "yyyyddMM-HHmmss"

Remove-Item $outputLocalNugetPath -Recurse -Force

dotnet pack ..\src\Prodigy.CQRS.Events\Prodigy.CQRS.Events.csproj --output $outputLocalNugetPath
dotnet pack ..\src\Prodigy.WebApi\Prodigy.WebApi.csproj --output $outputLocalNugetPath