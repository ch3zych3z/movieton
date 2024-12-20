module MovieTon.Parser.Core

open System.Collections.Generic

type 'a ParsingResult =
    | Success of 'a
    | ParsingError of string
    | UnknownError of obj

type internal ParsingResultBuilder() =
    member _.Bind(m, f) =
        match m with
        | Success a -> f a
        | ParsingError err -> ParsingError err
        | UnknownError err -> UnknownError err

    member _.Return(value) =
        Success value

    member _.ReturnFrom(mvalue: 'a ParsingResult) = mvalue

    member _.For(values, f) =
        let mappedValues = List()
        let mutable error = None
        for value in values do
            match error with
            | None ->
                match f value with
                | Success a -> mappedValues.Add(a)
                | ParsingError err -> error <- Some (ParsingError err)
                | UnknownError err -> error <- Some (UnknownError err)
            | Some _ -> ()

        Option.defaultValue (Success mappedValues) error

let internal parser = ParsingResultBuilder()
