module MovieTon.Parser.API

open System.Collections.Generic
open System.IO
open MovieTon.Core.Movie
open MovieTon.Core.Staff
open MovieTon.Core.Tag
open MovieTon.Parser.Core

type Config = {
    movieCodesPath: string
    actorsDirectorsNamesPath: string
    actorsDirectorsCodesPath: string
    ratingsPath: string
    linksPath: string
    tagCodesPath: string
    tagScoresPath: string
    relevanceLevel: float
}

type ParsedEntities = {
    titles: Title seq
    movies: Movie seq
    staffMembers: StaffMember seq
    participation: Participation seq
    tags: Tag seq
    movieTags: MovieTag seq
}

let private parseTitles config =
    Pipeline.run
        config.movieCodesPath
        Tokenizer.tokenizeTitle
        Parser.parseTitle

let private parseStaffMembers config =
    Pipeline.run
        config.actorsDirectorsNamesPath
        Tokenizer.tokenizeStaffMember
        Parser.parseStaffMember

let private parseParticipation config =
    Pipeline.run
        config.actorsDirectorsCodesPath
        Tokenizer.tokenizeParticipation
        Parser.parseParticipation

let private parseMovies config =
    Pipeline.run
        config.ratingsPath
        Tokenizer.tokenizeMovie
        Parser.parseMovie

let private parseTags config =
    Pipeline.run
        config.tagCodesPath
        Tokenizer.tokenizeTag
        Parser.parseTag

let private parseMovieTags config = parser {
    let! links =
        Pipeline.run
            config.linksPath
            Tokenizer.tokenizeLink
            Parser.parseLink
    let links = links |> Seq.map KeyValuePair |> Dictionary

    let! movieTags =
        Pipeline.run
            config.tagScoresPath
            Tokenizer.tokenizeMovieTag
            (Parser.parseMovieTags links)

    return Seq.choose id movieTags
}

let run config =
    try
        parser {
            let! titles = parseTitles config
            let! movies = parseMovies config
            let! staffMembers = parseStaffMembers config
            let! participation = parseParticipation config
            let! tags = parseTags config
            let! movieTags = parseMovieTags config

            return {
                titles = titles
                movies = movies
                staffMembers = staffMembers
                participation = participation
                tags = tags
                movieTags = movieTags
            }
        }
    with
         :? FileNotFoundException as exn -> ParsingError exn.Message
