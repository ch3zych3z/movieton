module internal MovieTon.Parser.Tokenizer

open System
open System.Collections.Generic

let private tokenize filePath tokenizer =
    filePath
    |> System.IO.File.ReadLines
    |> Seq.skip 1 // skip header
    |> Seq.iter tokenizer

let private indicesOfSep (sep: char) (str: string) =
    let spanInds = List()
    let mutable ind = 0
    spanInds.Add(-1)
    for c in str do
        if c = sep then
            spanInds.Add(ind)
        ind <- ind + 1
    spanInds.Add(str.Length)
    spanInds

let inline private spanStart (spanInds: List<int>) ind =
    spanInds[ind] + 1

let inline private spanLength (spanInds: List<int>) ind =
    spanInds[ind + 1] - spanInds[ind] - 1

let inline private substr (str: string) spans ind =
    str.Substring(spanStart spans ind, spanLength spans ind)

type TokenizedTitle = {
    title: string
    local: string
    movieId: string
}

let private tokenizeTitleHelper
    isAccepted
    (str: string)
    =
        let spans = indicesOfSep '\t' str
        let local =
            let region = substr str spans 3
            if isAccepted region then
                Some region
            else
                let language = substr str spans 4
                if isAccepted language then
                    Some language
                else
                    None
        if local.IsSome then
            let title = substr str spans 2
            let code = substr str spans 0
            Some {
                title = title
                local = local.Value
                movieId = code
            }
        else None

let tokenizeTitle str =
    let acceptedLocals = ["ru"; "en"; "us"; "gb"]
    let isAccepted str =
        acceptedLocals|> List.exists _.Equals(str, StringComparison.InvariantCultureIgnoreCase)
    tokenizeTitleHelper isAccepted str

type TokenizedStaffMember = {
    id: string
    name: string
}

let tokenizeStaffMember
    (str: string)
    =
        let spans = indicesOfSep '\t' str
        let code = substr str spans 0
        let name = substr str spans 1
        Some {
            id = code
            name = name
        }

type TokenizedParticipation = {
    staffId: string
    movieId: string
    role: string
}

let private tokenizeParticipationHelper
    isAccepted
    (str: string)
    =
        let spans = indicesOfSep '\t' str
        let role = substr str spans 3
        if isAccepted role then
            let movieCode = substr str spans 0
            let staffCode = substr str spans 2
            Some {
                staffId = staffCode
                movieId = movieCode
                role = role
            }
        else None

let tokenizeParticipation str =
    let acceptedRoles = ["actor"; "actress"; "director"]
    let isAccepted str =
        acceptedRoles |> List.exists _.Equals(str, StringComparison.InvariantCultureIgnoreCase)
    tokenizeParticipationHelper isAccepted str

type TokenizedMovie = {
    id: string
    rating: string
}

let tokenizeMovie
    (str: string)
    =
    let spans = indicesOfSep '\t' str
    let movieCode = substr str spans 0
    let rating = substr str spans 1
    Some {
        id = movieCode
        rating = rating
    }

type TokenizedLink = {
    movieLensId: string
    imdbId: string
}
let tokenizeLink
    (str: string)
    =
    let spans = indicesOfSep ',' str
    let mlCode = substr str spans 0
    let imdbId = substr str spans 1
    Some {
        movieLensId = mlCode
        imdbId = imdbId
    }

type TokenizedTag = {
    id: string
    name: string
}
let tokenizeTag
    (str: string)
    =
    let [| tagId; tag |] = str.Split(',')
    Some {
        id = tagId
        name = tag
    }

type TokenizedMovieTag = {
    tagId: string
    movieId: string
}

let tokenizeMovieTag
    isAccepted
    (str: string)
    =
    let spans = indicesOfSep ',' str
    let relevance = substr str spans 2
    if isAccepted relevance then
        let mlCode = substr str spans 0
        let tagId = substr str spans 1
        Some {
            tagId = tagId
            movieId = mlCode
        }
    else None
