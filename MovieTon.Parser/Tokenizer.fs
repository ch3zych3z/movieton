module internal MovieTon.Parser.Tokenizer

open System

let private tokenize filePath tokenizer =
    filePath
    |> System.IO.File.ReadLines
    |> Seq.skip 1 // skip header
    |> Seq.iter tokenizer

type TokenizedTitle = {
    title: string
    local: string
    movieId: string
}

let private tokenizeTitleHelper
    isAccepted
    (str: string)
    =
        let [| code; _; title; region; language; _; _; _ |] = str.Split('\t')
        let local =
            if isAccepted region then
                Some region
            elif isAccepted language then
                Some language
            else None
        if local.IsSome then
            Some {
                title = title
                local = local.Value
                movieId = code
            }
        else None

let tokenizeTitle str =
    let isAccepted str =
        ["ru"; "en"; "us"; "gb"] |> List.exists _.Equals(str, StringComparison.InvariantCultureIgnoreCase)
    tokenizeTitleHelper isAccepted str

type TokenizedStaffMember = {
    id: string
    name: string
}

let tokenizeStaffMember
    (str: string)
    =
        let [| code; name; _; _; _; _ |] = str.Split('\t')
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
        let [| movieCode; _; staffCode; role; _; _|] = str.Split('\t')
        if isAccepted role then
            Some {
                staffId = staffCode
                movieId = movieCode
                role = role
            }
        else None

let tokenizeParticipation str =
    let isAccepted str =
        ["actor"; "actress"; "director"] |> List.exists _.Equals(str, StringComparison.InvariantCultureIgnoreCase)
    tokenizeParticipationHelper isAccepted str

type TokenizedMovie = {
    id: string
    rating: string
}

let tokenizeMovie
    (str: string)
    =
    let [| movieCode; rating; _ |] = str.Split('\t')
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
    let [| mlCode; imdbId; _ |] = str.Split(',')
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
    relevance: string
}

let tokenizeMovieTag
    (str: string)
    =
    let [| mlCode; tagId; relevance |] = str.Split(',')
    Some {
        tagId = tagId
        movieId = mlCode
        relevance = relevance
    }
