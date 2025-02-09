open System
open Microsoft.Extensions.Configuration

open MovieTon
open MovieTon.CLIRunner
open MovieTon.Core.App
open MovieTon.Core.Movie
open MovieTon.CLIRunner.Interpreter

let private mkInterpreter (config: IConfiguration) =
    let connectionString = config.GetSection("Database")["ConnectionString"]
    let repo = MovieTon.Database.Api.Repository.make connectionString
    let titleViewStrategy titles =
        Seq.tryFind (fun it -> it.local = Localization.Ru) titles
        |> Option.orElse (Seq.tryHead titles)
        |> Option.map _.title
        |> Option.defaultValue "Unknown title"
    let app = MovieTonApp(repo, titleViewStrategy)
    Interpreter(app)

let private readCmd () =
    Console.Write("> ")
    Console.ReadLine()

let rec private step (interpreter: Interpreter) lastCode =
    if lastCode = 0 then
        let strCmd = readCmd ()
        if strCmd <> null then
            match strCmd |> Parser.Command.parse with
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
    let config = ConfigurationBuilder().AddJsonFile("appsettings.json").Build()
    let interpreter = mkInterpreter config

    Logger.debug "Started!"
    step interpreter 0
