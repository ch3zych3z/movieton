module internal MovieTon.Parser.Parser

open System.Collections.Generic
open System.IO
open MovieTon.Parser.Tokenizer
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

let parseTitle (token: TokenizedTitle) = parser {
    let! parsedCode = parseCode "tt" token.movieId
    let local = Localization.UnsafeParse token.local
    return Title.Of token.title local parsedCode
}

let parseStaffMember (token: TokenizedStaffMember) = parser {
    let! parsedCode = parseCode "nm" token.id
    return StaffMember.Of parsedCode token.name
}

let parseParticipation (token: TokenizedParticipation) = parser {
    let! staffCode = parseCode "nm" token.staffId
    let! movieCode = parseCode "tt" token.movieId
    let roles = Role.Parse token.role
    return Participation.Of staffCode movieCode roles
}

let parseMovie (token: TokenizedMovie) = parser {
    let! movieCode = parseCode "tt" token.id
    let! rating = parseRating token.rating
    return Movie.Of movieCode rating
}

let parseLink (token: TokenizedLink) = parser {
    let! id = parseInt token.movieLensId
    let! imdbId = parseInt token.imdbId
    return id, imdbId
}

let parseTag (token: TokenizedTag) = parser {
    let! tagId = parseInt token.id
    return Tag.Of tagId token.name
}

let parseMovieTags (links: Dictionary<int, int>) (token: TokenizedMovieTag) = parser {
    let! movieLensId = parseInt token.movieId
    let! tagId = parseInt token.tagId
    let! relevance = parseFloat token.relevance

    let movieId = links[movieLensId]

    if relevance > 0.5 then
        return MovieTag.Of tagId movieId |> Some
    else return None
}

// let parseEntities config =
//     try
//         parser {
//             let! titles = parseTitle config
//             let! staffMembers = parseStaffMembers config
//             let! participation = parseParticipation config
//             let! movies = parseMovies config
//             let! tags = parseTags config
//             let! movieTags = parseMovieTags config
//             return {
//                 titles = titles
//                 staffMembers = staffMembers
//                 participation = participation
//                 movies = movies
//                 tags = tags
//                 movieTags = movieTags
//             }
//         }
//     with
//         :? FileNotFoundException as exn -> ParsingError exn.Message
