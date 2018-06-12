@echo off

SET VERSION=0.0.0
IF NOT [%1]==[] (set VERSION=%1)

dotnet pack src/Tools.SerilogConfiguration/Tools.SerilogConfiguration.csproj -o ../../dist -p:Version="%VERSION%" -p:PackageVersion="%VERSION%" -c Release