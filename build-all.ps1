$version = "0.0.0"

if($args[0]) { $version=$args[0]}

Write-Host "Building with Version Number: " $Version

Function Build-Fake([string]$folderName){
    pushd $folderName
    ./build.cmd package release $version
    popd

    cp $folderName\dist\*.zip .\dist
    cp $folderName\dist\*.nupkg .\dist

    if($LastExitCode -ne 0){
        throw "$folderName build failed"
    }
}

rmdir -R dist
mkdir dist

Build-Fake "src"
