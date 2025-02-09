module MovieTon.Parser.Integrity

open System.Collections.Generic
open MovieTon
open MovieTon.Core.Movie
open MovieTon.Parser.Parser

let private uniqueCheck (entities: ParsedEntities) =
    let unique (x: _ seq) = HashSet(x)

    {
        titles = unique entities.titles
        movies = entities.movies
        staffMembers = entities.staffMembers
        participation = unique entities.participation
        tags = entities.tags
        movieTags = entities.movieTags
    }

let private idCheck (entities: ParsedEntities) =
    let movieIds = entities.movies |> Seq.map _.id |> HashSet
    movieIds.IntersectWith(entities.titles |> Seq.map _.movieId)
    let staffIds = entities.staffMembers |> Seq.map _.id |> HashSet
    let tagIds = entities.tags |> Seq.map _.id |> HashSet

    { entities with
        movies = entities.movies |> Seq.filter (fun m -> movieIds.Contains(m.id))
        titles = entities.titles |> Seq.filter (fun t -> movieIds.Contains(t.movieId))
        participation = entities.participation |> Seq.filter (fun p -> movieIds.Contains(p.movieId) && staffIds.Contains(p.staffId))
        movieTags = entities.movieTags |> Seq.filter (fun t -> movieIds.Contains(t.movieId) && tagIds.Contains(t.tagId))
    }

let private titleCheck (entities: ParsedEntities) =
    let titlesCache = HashSet<int * Localization>()
    { entities with
        titles = entities.titles |> List.ofSeq |> List.filter (fun t -> titlesCache.Add(t.movieId, t.local))
    }

let integrityFilter = uniqueCheck >> idCheck >> titleCheck
