module MovieTon.CLIRunner.Similarity

open System.Collections.Generic
open MovieTon
open MovieTon.Parser.Parser
open MovieTon.Utils.Collections
open MovieTon.Core.Similarity

type private Cache = {
    movie2tags: Dictionary<int, HashSet<int>>
    movie2staff: Dictionary<int, HashSet<int>>

    tag2movies: Dictionary<int, HashSet<int>>
    staff2movies: Dictionary<int, HashSet<int>>

    movie2rating: Dictionary<int, float>
}
with
    static member Empty() = {
        movie2tags = Dictionary()
        movie2staff = Dictionary()
        tag2movies = Dictionary()
        staff2movies = Dictionary()
        movie2rating = Dictionary()
    }

let private mkCache (entities: ParsedEntities) =
    let cache = Cache.Empty()

    for tag in entities.movieTags do
        let tags = Dictionary.getOrCompute tag.movieId HashSet cache.movie2tags
        tags.Add(tag.tagId) |> ignore

        let movies = Dictionary.getOrCompute tag.tagId HashSet cache.tag2movies
        movies.Add(tag.movieId) |> ignore

    for staff in entities.participation do
        let stf = Dictionary.getOrCompute staff.movieId HashSet cache.movie2staff
        stf.Add(staff.staffId) |> ignore

        let movies = Dictionary.getOrCompute staff.staffId HashSet cache.staff2movies
        movies.Add(staff.movieId) |> ignore

    for movie in entities.movies do
        cache.movie2rating[movie.id] <- float movie.rating / 100.0

    cache

let private similarity cache movieId similarId =
    let fromStaff =
        let movieStaff = Dictionary.get movieId cache.movie2staff
        let similarStaff = Dictionary.get similarId cache.movie2staff
        match movieStaff, similarStaff with
        | Some movieStaff, Some similarStaff ->
            let staffIntersection = HashSet(movieStaff)
            let sz = float staffIntersection.Count
            staffIntersection.IntersectWith(similarStaff)
            float staffIntersection.Count / sz
        | _ -> 0.0

    let fromTags =
        let movieTags = Dictionary.get movieId cache.movie2tags
        let similarTags = Dictionary.get similarId cache.movie2tags
        match movieTags, similarTags with
        | Some movieTags, Some similarTags ->
            let tagsIntersection = HashSet(movieTags)
            let sz = float tagsIntersection.Count
            tagsIntersection.IntersectWith(similarTags)
            float tagsIntersection.Count / sz
        | _ -> 0.0

    let fromRating = cache.movie2rating[similarId]

    let score =
        0.5 * fromRating
        + 0.25 * fromStaff
        + 0.25 * fromTags
    {
        similarId = similarId
        movieId = movieId
        confidence = min 1.0 score
    }

let getSimilarities (entities: ParsedEntities) =
    let cache = mkCache entities
    Logger.debug "cache computed..."
    [
    for movie in entities.movies do
        let movieId = movie.id
        let similars = HashSet()
        if cache.movie2tags.ContainsKey(movieId) then
            for tag in cache.movie2tags[movieId] do
                similars.UnionWith(cache.tag2movies[tag])
        if cache.movie2staff.ContainsKey(movieId) then
            for staff in cache.movie2staff[movieId] do
                similars.UnionWith(cache.staff2movies[staff])

        let similars =
            let result = List()
            for similar in similars do
                result.Add(similarity cache movieId similar)
            result.Sort()
            result

        similars.Reverse()
        yield! Seq.truncate 10 similars
    ]
