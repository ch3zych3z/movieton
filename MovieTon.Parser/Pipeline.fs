module internal MovieTon.Parser.Pipeline

open System.Collections.Concurrent
open System.IO
open System.Threading.Tasks

open MovieTon.Parser.Core

let runTask filepath tokenize parse =
    let lines = new BlockingCollection<string>()
    let tokens = new BlockingCollection<_>()

    let readTask = Task.Run(fun () ->
            let fileReadStream = File.OpenRead(filepath)
            use reader = new StreamReader(fileReadStream)
            let mutable line = reader.ReadLine()
            line <- reader.ReadLine()
            while line <> null do
                lines.Add(line)
                line <- reader.ReadLine()
            lines.CompleteAdding()
        )

    let tokenizationTask = Task.Run(fun () ->
            lines.GetConsumingEnumerable()
            |> Seq.iter (tokenize >> Option.iter tokens.Add)
            tokens.CompleteAdding()
        )

    let parsingTask = Task.Run(fun () ->
            parser {
                for token in tokens.GetConsumingEnumerable() do return! parse token
            }
        )

    task {
        do! readTask
        do! tokenizationTask
        let! parsingResult = parsingTask
        lines.Dispose()
        tokens.Dispose()
        return parsingResult
    }

let run filepath tokenize parse =
    let task = runTask filepath tokenize parse
    task.Wait()
    task.Result
