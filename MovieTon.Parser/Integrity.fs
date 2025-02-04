module MovieTon.Parser.Integrity

open System.Collections.Generic
open MovieTon.Core.Movie
open MovieTon.Parser.Parser

let private uniqueCheck (entities: ParsedEntities) =
    let unique x = Set.ofSeq x |> List.ofSeq |> Seq.cast
    {
        titles = unique entities.titles
        movies = unique entities.movies
        staffMembers = unique entities.staffMembers
        participation = unique entities.participation
        tags = unique entities.tags
        movieTags = unique entities.movieTags
    }

let private idCheck (entities: ParsedEntities) =
    let movieIds = entities.movies |> Seq.map _.id |> Set.ofSeq
    let staffIds = entities.staffMembers |> Seq.map _.id |> Set.ofSeq
    let tagIds = entities.tags |> Seq.map _.id |> Set.ofSeq

    { entities with
        titles = entities.titles |> Seq.filter (fun t -> Set.contains t.movieId movieIds)
        participation = entities.participation |> Seq.filter (fun p -> Set.contains p.movieId movieIds && Set.contains p.staffId staffIds)
        movieTags = entities.movieTags |> Seq.filter (fun t -> Set.contains t.movieId movieIds && Set.contains t.tagId tagIds)
    }

let private titleCheck (entities: ParsedEntities) =
    let titlesCache = HashSet<int * Localization>()
    { entities with
        titles = entities.titles |> List.ofSeq |> List.filter (fun t -> titlesCache.Add(t.movieId, t.local))
    }

let integrityFilter = uniqueCheck >> idCheck >> titleCheck
