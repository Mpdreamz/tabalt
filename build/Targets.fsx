#I @"../packages/build/FAKE/tools" 
#r @"FakeLib.dll"
open System
open Fake 

Target "Build" <| fun _ -> traceHeader "STARTING BUILD"

"Build"

RunTargetOrDefault "Build"
