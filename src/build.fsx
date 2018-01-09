#r "packages/FAKE/tools/FakeLib.dll"
#load "../build-common.fsx"

open Fake
open build.common

let buildDir = "./build/"
let packageDir = "./dist/"

let projectName = "cr-logging"
let projectFolder = buildDir + projectName

let zipPath name = packageDir + name + "-" + version + ".zip"

Target "Clean" (fun _ ->
    CleanDirs [packageDir]
)

Target "Package-Project" (fun _ ->
    let nugetPackageName = "CR.Logging"
    let nugetPackageDescription = "A wrapper for initialising Serilog consistently."

    zipPackage projectFolder (zipPath projectName)
    nugetPackage projectFolder nugetPackageName nugetPackageDescription packageDir
)

Target "Post-Clean" (fun _ ->
    CleanDirs [buildDir]
)


Target "Package" DoNothing
Target "Build" DoNothing
Target "Default" DoNothing

"Clean"
    ==> "RestorePackages"
    ==> "Build"

"Build"
    ==> "Package-Project"
    ==> "Post-Clean"
    ==> "Package"

"Package"
    ==> "Default"

RunTargetOrDefault "Default"
