$outputLocalNugetPath = "C:\Users\daniel.mrowca\.nuget\localBuildsPackages";
$todayDate = Get-Date -Format "yyyyddMM-HHmmss"

Remove-Item $outputLocalNugetPath -Recurse -Force

dotnet pack ..\src\HoneyComb.CQRS.Events\HoneyComb.CQRS.Events.csproj --output $outputLocalNugetPath
dotnet pack ..\src\HoneyComb.WebApi\HoneyComb.WebApi.csproj --output $outputLocalNugetPath