module MovieTon.Parser.Primitives

open System.Globalization
open MovieTon.Parser.Core

open System

let parseInt (str: string) =
    match Int32.TryParse str with
    | true, parsed -> Success parsed
    | _ -> ParsingError $"{str} is not int"

let parseFloat (str: string) =
    match Double.TryParse(str, CultureInfo.InvariantCulture) with
    | true, parsed -> Success parsed
    | _ -> ParsingError $"{str} is not float"

let parseList p elements = parser {
    for e in elements do
        return! p e
}

let assume pred str =
    if pred str then Success ()
    else ParsingError $"Given string \"{str}\" violates the assumption"
