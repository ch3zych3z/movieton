module internal MovieTon.Parser.Tokenizer

open System
open System.Collections.Generic

let private tokenize filePath tokenizer =
    filePath
    |> System.IO.File.ReadLines
    |> Seq.skip 1 // skip header
    |> Seq.iter tokenizer

let private cacheMovieByLanguage
    (codes: List<string>)
    (locals: List<string>)
    (titles: List<string>)
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
            codes.Add(code)
            titles.Add(title)
            locals.Add(local.Value)

let tokenizeMovieCodes filePath =
    let codes = List()
    let locals = List()
    let titles = List()

    let isAccepted str =
        ["ru"; "en"; "us"; "gb"] |> List.exists _.Equals(str, StringComparison.InvariantCultureIgnoreCase)
    cacheMovieByLanguage codes locals titles isAccepted
    |> tokenize filePath

    codes, locals, titles

let private cacheActorsDirectors
    (codes: List<string>)
    (names: List<string>)
    (str: string)
    =
        let [| code; name; _; _; _; _ |] = str.Split('\t')
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
        let [| movieCode; _; staffCode; role; _; _|] = str.Split('\t')
        if isAccepted role then
            movies.Add(movieCode)
            staff.Add(staffCode)
            roles.Add(role)

let tokenizeActorsDirectorsCodes filePath =
    let movieCodes = List()
    let staffCodes = List()
    let roles = List()

    let isAccepted str =
        ["actor"; "actress"; "director"] |> List.exists _.Equals(str, StringComparison.InvariantCultureIgnoreCase)
    cacheStaffMoviesNRoles movieCodes staffCodes roles isAccepted
    |> tokenize filePath

    movieCodes, staffCodes, roles

let private cacheMovieRatings
    (movies: List<string>)
    (ratings: List<string>)
    (str: string)
    =
    let [| movieCode; rating; _ |] = str.Split('\t')
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
    let [| mlCode; imdbId; _ |] = str.Split(',')
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
    (relevances: List<string>)
    (str: string)
    =
    let [| mlCode; tagId; relevance |] = str.Split(',')
    mlCodes.Add(mlCode)
    tagIds.Add(tagId)
    relevances.Add(relevance)

let tokenizeTagScores filePath =
    let mlCodes = List()
    let tagIds = List()
    let relevances = List()

    cacheRelevance mlCodes tagIds relevances
    |> tokenize filePath

    mlCodes, tagIds, relevances
