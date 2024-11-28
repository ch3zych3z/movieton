module MovieTon.CLIRunner.Parser

open MovieTon.CLIRunner.Interpreter
open MovieTon.Parser
open MovieTon.Parser.Parser
open MovieTon.Parser.Core

module Command =
    let private mkInfoCmd cmd v =
        v |> String.concat " " |> cmd |> Info |> Ok

    let parse (str: string) =
        match str.Split() |> List.ofArray with
        | "info" :: "movie" :: title -> title |> mkInfoCmd MovieInfo
        | "info" :: "staff" :: name -> name |> mkInfoCmd StaffInfo
        | "info" :: "tag" :: name -> name |> mkInfoCmd TagInfo
        | _ -> Error "Unknown command. Use `help` to list supported commands"

    let parsingCommand config =
        {
            parse = fun () -> Parser.parseEntities config :> obj
            doAfter = fun res ->
                match res with
                | :? ParsingResult<ParsedEntities> as res ->
                    match res with
                    | Success entities -> [
                            entities.movies |> PutMovies |> Put
                            entities.titles |> PutTitles |> Put
                            entities.staffMembers |> PutStaffMembers |> Put
                            entities.participation |> PutParticipation |> Put
                            entities.tags |> PutTags |> Put
                            entities.movieTags |> PutMovieTags |> Put
                        ]
                    | ParsingError msg -> [ Print msg; Exit 1]
                    | UnknownError obj -> [ Print $"{obj}"; Exit 2 ]
                | _ -> [Exit 42]
        } |> Parse

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
