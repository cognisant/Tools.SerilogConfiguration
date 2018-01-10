#r "packages/FAKE/tools/FakeLib.dll"
#load "../build-common.fsx"

open Fake
open build.common

let buildDir = "./build/"
let packageDir = "./dist/"

let projectName = "cr-logging"
let projectFolder = buildDir + projectName

let projectNameES = "cr-logging.eventstore"
let projectFolderES = buildDir + projectNameES

let zipPath name = packageDir + name + "-" + version + ".zip"

Target "Clean" (fun _ ->
    CleanDirs [packageDir]
)

Target "Post-Clean" (fun _ ->
    CleanDirs [buildDir]
)


Target "Package" DoNothing
Target "Build" DoNothing
Target "Default" DoNothing

"Clean"
    ==> "RestorePackages"

"Build"
    ==> "Post-Clean"
    ==> "Package"

"Package"
    ==> "Default"

RunTargetOrDefault "Default"
