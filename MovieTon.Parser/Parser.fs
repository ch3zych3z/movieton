module MovieTon.Parser.Parser

open System.IO
open MovieTon.Utils.Collections
open MovieTon.Core.Staff
open MovieTon.Core.Movie
open MovieTon.Parser.Core
open MovieTon.Parser.Primitives
open MovieTon.Core.Tag

let private parseCode (prefix: string) (str: string) = parser {
    do! str |> assume _.StartsWith(prefix)
    let codePart = str.Substring(2)
    return! parseInt codePart
}

let private parseRating (str: string) = parser {
    do! str |> assume _.Contains('.')
    let [| d1; d2 |] = str.Split('.')
    let! d1p = parseInt d1
    let! d2p = parseInt d2
    return d1p * 10 + d2p
}

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

let private parseTitles config = parser {
    let codes, locals, titles = Tokenizer.tokenizeMovieCodes config.movieCodesPath
    let! parsedCodes = parseList (parseCode "tt") codes
    let locals = Seq.map Localization.UnsafeParse locals
    return Seq.map3 Title.Of titles locals parsedCodes
}

let private parseStaffMembers config = parser {
    let codes, names = Tokenizer.tokenizeActorsDirectorsNames config.actorsDirectorsNamesPath
    let! parsedCodes = parseList (parseCode "nm") codes
    return Seq.map2 StaffMember.Of parsedCodes names
}

let private parseParticipation config = parser {
    let movieCodes, staffCodes, roles = Tokenizer.tokenizeActorsDirectorsCodes config.actorsDirectorsCodesPath
    let! staffCodes = parseList (parseCode "nm") staffCodes
    let! movieCodes = parseList (parseCode "tt") movieCodes
    let roles = Seq.map Role.Parse roles
    return Seq.map3 Participation.Of staffCodes movieCodes roles
}

let private parseMovies config = parser {
    let codes, ratings = Tokenizer.tokenizeRatings config.ratingsPath
    let! movieCodes = parseList (parseCode "tt") codes
    let! ratings = parseList parseRating ratings
    return Seq.map2 Movie.Of movieCodes ratings
}

let private parseLinks config = parser {
    let ids, imdbIds = Tokenizer.tokenizeLinks config.linksPath
    let! ids = parseList parseInt ids
    let! imdbIds = parseList parseInt imdbIds
    return Dictionary.init2 ids imdbIds
}

let private parseTags config = parser {
    let tagIds, tags = Tokenizer.tokenizeTagCodes config.tagCodesPath
    let! tagIds = parseList parseInt tagIds
    return Seq.map2 Tag.Of tagIds tags
}

let private parseMovieTags config = parser {
    let isAccepted relevance =
        let res =
            parser {
                let! relevance = parseFloat relevance
                return relevance > config.relevanceLevel
            }
        match res with
        | Success true -> true
        | _ -> false

    let movieLensIds, tagIds = Tokenizer.tokenizeTagScores config.tagScoresPath isAccepted
    let! movieLensIds = parseList parseInt movieLensIds
    let! tagIds = parseList parseInt tagIds

    let! movieLensId2imdb = parseLinks config
    let movieIds = Seq.map (fun mlId -> movieLensId2imdb[mlId]) movieLensIds

    return Seq.map2 MovieTag.Of tagIds movieIds
}

let parseEntities config =
    try
        parser {
            let! titles = parseTitles config
            let! staffMembers = parseStaffMembers config
            let! participation = parseParticipation config
            let! movies = parseMovies config
            let! tags = parseTags config
            let! movieTags = parseMovieTags config
            return {
                titles = titles
                staffMembers = staffMembers
                participation = participation
                movies = movies
                tags = tags
                movieTags = movieTags
            }
        }
    with
        :? FileNotFoundException as exn -> ParsingError exn.Message
