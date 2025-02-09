module MovieTon.Logger

open System

type LogLevel =
    | Debug = 1
    | Trace = 2
    | Info = 3
    | Error = 4

let mutable private currentLogLevel = LogLevel.Info

let setLogLevel level = currentLogLevel <- level

let private toString level =
    match level with
    | LogLevel.Debug -> "Debug"
    | LogLevel.Trace -> "Trace"
    | LogLevel.Info -> "Info"
    | LogLevel.Error -> "Error"

let private log level msg =
    if level >= currentLogLevel then
        printfn $"[{toString level}][{DateTime.Now}] {msg}"

let debug msg = log LogLevel.Debug msg
let trace msg = log LogLevel.Trace msg
let info msg = log LogLevel.Info msg
let error msg = log LogLevel.Error msg
