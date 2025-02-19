module MovieTon.Parser.API

open MovieTon.Parser.Core
open MovieTon.Parser.Parser

open System.Collections.Generic
open System.IO
open System.Threading.Tasks

let private parseTitles config =
    Pipeline.runTask
        config.movieCodesPath
        Tokenizer.tokenizeTitle
        Parser.parseTitle

let private parseStaffMembers config =
    Pipeline.runTask
        config.actorsDirectorsNamesPath
        Tokenizer.tokenizeStaffMember
        Parser.parseStaffMember

let private parseParticipation config =
    Pipeline.runTask
        config.actorsDirectorsCodesPath
        Tokenizer.tokenizeParticipation
        Parser.parseParticipation

let private parseMovies config =
    Pipeline.runTask
        config.ratingsPath
        Tokenizer.tokenizeMovie
        Parser.parseMovie

let private parseTags config =
    Pipeline.runTask
        config.tagCodesPath
        Tokenizer.tokenizeTag
        Parser.parseTag

let private isAcceptedMovieTag config str =
    match Primitives.parseFloat str with
    | Success relevance when relevance > config.relevanceLevel -> true
    | _ -> false

let private parseMovieTags config = task {
    let! linksTask =
        Pipeline.runTask
            config.linksPath
            Tokenizer.tokenizeLink
            Parser.parseLink
    let movieTagsTask =
        parser {
            let! links = linksTask
            let links = links |> Seq.map KeyValuePair |> Dictionary
            return Pipeline.runTask
                config.tagScoresPath
                (Tokenizer.tokenizeMovieTag (isAcceptedMovieTag config))
                (Parser.parseMovieTags links)
        }
    return!
        match movieTagsTask with
        | Success task -> task
        | ParsingError msg -> ParsingError msg |> Task.FromResult
        | UnknownError err -> UnknownError err |> Task.FromResult
}

let run config =
    try
        let titles = parseTitles config
        let movies = parseMovies config
        let staffMembers = parseStaffMembers config
        let participation = parseParticipation config
        let tags = parseTags config
        let movieTags = parseMovieTags config
        let parsingTask =
            task {
                let! titles = titles
                let! movies = movies
                let! staffMembers = staffMembers
                let! participation = participation
                let! tags = tags
                let! movieTags = movieTags
                return parser {
                    let! titles = titles
                    let! movies = movies
                    let! staffMembers = staffMembers
                    let! participation = participation
                    let! tags = tags
                    let! movieTags = movieTags
                    return {
                        titles = titles
                        movies = movies
                        staffMembers = staffMembers
                        participation = participation
                        tags = tags
                        movieTags = movieTags
                    }
                }
            }
        parsingTask.Wait()
        parsingTask.Result
    with
         :? FileNotFoundException as exn -> ParsingError exn.Message
