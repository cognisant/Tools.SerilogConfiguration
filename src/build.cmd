@echo on

If NOT exist "./packages/FAKE/tools/Fake.exe" (
".nuget/NuGet.exe" "Install" "FAKE" "-OutputDirectory" "packages" "-ExcludeVersion"
)

SET TARGET="Default"
IF NOT [%1]==[] (set TARGET="%1")

SET BUILDMODE="Release"
IF NOT [%2]==[] (set BUILDMODE="%2")

SET VERSION=0.0.0
IF NOT [%3]==[] (set VERSION=%3)

dotnet publish -p:PackageVersion=%VERSION% cr-logging -o ../build/cr-logging -c Release --self-contained -r -win-x86
if %errorlevel% neq 0 exit /b %errorlevel%

"packages/FAKE/tools/Fake.exe" build.fsx "target=%TARGET%" "buildMode=%BUILDMODE%" "version=%VERSION%"
if %errorlevel% neq 0 exit /b %errorlevel%
