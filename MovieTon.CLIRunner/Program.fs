open System
open MovieTon.CLIRunner
open MovieTon.Core.App
open MovieTon.Core.Movie
open MovieTon.CLIRunner.Interpreter

let private path = "/Users/chez/sources/spbu/MovieTon/ml-latest"

let private mkInterpreter () =
    let movieRepo, staffRepo, tagRepo = Repository.emptyRepositories ()
    let titleViewStrategy titles =
        Seq.tryFind (fun it -> it.local = Localization.Ru) titles
        |> Option.defaultValue (Seq.head titles)
        |> _.title
    let app = MovieTonApp(movieRepo, staffRepo, tagRepo, titleViewStrategy)
    Interpreter(app)

let private readCmd () =
    Console.Write("> ")
    Console.ReadLine()

let rec private step (interpreter: Interpreter) lastCode =
    if lastCode = 0 then
        let strCmd = readCmd ()
        if strCmd <> null then
            match readCmd () |> Parser.Command.parse with
            | Ok cmd ->
                let code = interpreter.Interpret cmd
                step interpreter code
            | Error str ->
                printfn $"{str}"
                step interpreter lastCode
        else 0
    else lastCode

[<EntryPoint>]
let main _ =
    let config = Parser.Config.singleDirectoryDefaultConfig path
    let interpreter = mkInterpreter ()

    Parser.Command.parsingCommand config
    |> interpreter.Interpret
    |> step interpreter
