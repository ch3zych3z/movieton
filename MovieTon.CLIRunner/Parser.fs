module MovieTon.CLIRunner.Parser

open MovieTon.CLIRunner.Interpreter
open MovieTon
open MovieTon.Parser.Parser
open MovieTon.Parser.Core

module Config =
    let singleDirectoryDefaultConfig path =
        let sep = System.IO.Path.DirectorySeparatorChar
        {
            movieCodesPath = $"{path}{sep}MovieCodes_IMDB.tsv"
            actorsDirectorsNamesPath = $"{path}{sep}ActorsDirectorsNames_IMDB.txt"
            actorsDirectorsCodesPath = $"{path}{sep}ActorsDirectorsCodes_IMDB.tsv"
            ratingsPath = $"{path}{sep}Ratings_IMDB.tsv"
            linksPath = $"{path}{sep}links_IMDB_MovieLens.csv"
            tagCodesPath = $"{path}{sep}TagCodes_MovieLens.csv"
            tagScoresPath = $"{path}{sep}TagScores_MovieLens.csv"
            relevanceLevel = 0.5
        }

module Command =
    let private mkInfoCmd cmd v =
        v |> String.concat " " |> cmd |> Info |> Ok

    let private doAfter (res: ParsingResult<ParsedEntities>) =
        match res with
        | Success entities ->
            let entities = MovieTon.Parser.Integrity.integrityFilter entities
            [
                yield entities.movies |> PutMovies
                yield entities.titles |> PutTitles
                yield entities.staffMembers |> PutStaffMembers
                yield entities.participation |> PutParticipation
                yield entities.tags |> PutTags
                yield entities.movieTags |> PutMovieTags
            ] |> List.map Put
        | ParsingError msg -> [ Print msg; Exit 1]
        | UnknownError obj -> [ Print $"{obj}"; Exit 2 ]

    let parsingCommand config =
        {
            parse = fun () -> Parser.API.run config :> obj
            doAfter = fun res ->
                match res with
                | :? ParsingResult<ParsedEntities> as res -> doAfter res
                | _ -> [Exit 42]
        } |> Parse

    let parse (str: string) =
        match str.Split() |> List.ofArray with
        | "info" :: "movie" :: title -> title |> mkInfoCmd MovieInfo
        | "info" :: "staff" :: name -> name |> mkInfoCmd StaffInfo
        | "info" :: "tag" :: name -> name |> mkInfoCmd TagInfo
        | "parse" :: path ->
            String.concat " " path
            |> Config.singleDirectoryDefaultConfig
            |> parsingCommand
            |> Ok
        | _ -> Error "Unknown command. Use `help` to list supported commands"
