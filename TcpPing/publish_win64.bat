cd /d %~dp0
dotnet publish -c Release -r win-x64 -p:PublishAot=true
pause