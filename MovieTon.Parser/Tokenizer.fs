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

let inline private span (str: string) spans ind =
    str.AsSpan(spanStart spans ind, spanLength spans ind)

let inline private substr (str: string) spans ind =
    str.Substring(spanStart spans ind, spanLength spans ind)

let private cacheMovieByLanguage
    (codes: List<string>)
    (locals: List<string>)
    (titles: List<string>)
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
            codes.Add(code)
            titles.Add(title)
            locals.Add(local.Value)

let tokenizeMovieCodes filePath =
    let codes = List()
    let locals = List()
    let titles = List()

    let acceptedLocals = ["ru"; "en"; "us"; "gb"]
    let isAccepted str =
        acceptedLocals |> List.exists _.Equals(str, StringComparison.InvariantCultureIgnoreCase)
    cacheMovieByLanguage codes locals titles isAccepted
    |> tokenize filePath

    codes, locals, titles

let private cacheActorsDirectors
    (codes: List<string>)
    (names: List<string>)
    (str: string)
    =
        let spans = indicesOfSep '\t' str
        let code = substr str spans 0
        let name = substr str spans 1
        codes.Add(code)
        names.Add(name)

let tokenizeActorsDirectorsNames filePath =
    let codes = List()
    let names = List()

    cacheActorsDirectors codes names
    |> tokenize filePath

    codes, names

let private cacheStaffMoviesNRoles
    (movies: List<string>)
    (staff: List<string>)
    (roles: List<string>)
    isAccepted
    (str: string)
    =
        let spans = indicesOfSep '\t' str
        let role = substr str spans 3
        if isAccepted role then
            let movieCode = substr str spans 0
            let staffCode = substr str spans 2
            movies.Add(movieCode)
            staff.Add(staffCode)
            roles.Add(role)

let tokenizeActorsDirectorsCodes filePath =
    let movieCodes = List()
    let staffCodes = List()
    let roles = List()

    let acceptedRoles = ["actor"; "actress"; "director"]
    let isAccepted str =
        acceptedRoles |> List.exists _.Equals(str, StringComparison.InvariantCultureIgnoreCase)
    cacheStaffMoviesNRoles movieCodes staffCodes roles isAccepted
    |> tokenize filePath

    movieCodes, staffCodes, roles

let private cacheMovieRatings
    (movies: List<string>)
    (ratings: List<string>)
    (str: string)
    =
    let spans = indicesOfSep '\t' str
    let movieCode = substr str spans 0
    let rating = substr str spans 1
    movies.Add(movieCode)
    ratings.Add(rating)

let tokenizeRatings filePath =
    let movieCodes = List()
    let ratings = List()

    cacheMovieRatings movieCodes ratings
    |> tokenize filePath

    movieCodes, ratings

let private cacheLinks
    (mlCodes: List<string>)
    (imdbIds: List<string>)
    (str: string)
    =
    let spans = indicesOfSep ',' str
    let mlCode = substr str spans 0
    let imdbId = substr str spans 1
    mlCodes.Add(mlCode)
    imdbIds.Add(imdbId)

let tokenizeLinks filePath =
    let mlCodes = List()
    let imdbIds = List()

    cacheLinks mlCodes imdbIds
    |> tokenize filePath

    mlCodes, imdbIds

let private cacheTagNCodes
    (tagCodes: List<string>)
    (tags: List<string>)
    (str: string)
    =
    let [| tagId; tag |] = str.Split(',')
    tagCodes.Add(tagId)
    tags.Add(tag)

let tokenizeTagCodes filePath =
    let tagCodes = List()
    let tags = List()

    cacheTagNCodes tagCodes tags
    |> tokenize filePath

    tagCodes, tags

let private cacheRelevance
    (mlCodes: List<string>)
    (tagIds: List<string>)
    isAccepted
    (str: string)
    =
    let spans = indicesOfSep ',' str
    let relevance = substr str spans 2
    if isAccepted relevance then
        let mlCode = substr str spans 0
        let tagId = substr str spans 1
        mlCodes.Add(mlCode)
        tagIds.Add(tagId)

let tokenizeTagScores filePath isAccepted =
    let mlCodes = List()
    let tagIds = List()

    cacheRelevance mlCodes tagIds isAccepted
    |> tokenize filePath

    mlCodes, tagIds
