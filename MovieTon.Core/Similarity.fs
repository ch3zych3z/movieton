module MovieTon.Core.Similarity

open System
open System.Collections.Generic
open MovieTon.Core.Info

[<CustomComparison; StructuralEquality>]
type Similarity = {
    movieId: int
    similarId: int
    confidence: float
}
with
    interface IComparable<Similarity> with
        member x.CompareTo(other) = x.confidence - other.confidence |> int


let similarityConfidence (movie: MovieInfo) (similar: MovieInfo) =
    let fromRating = float similar.rating / 100.0

    let fromParticipants =
        let participants (info: MovieInfo) =
            let p = HashSet(info.actors)
            match info.director with
            | Some director -> p.Add(director) |> ignore
            | _ -> ()
            p
        let movieParticipants = participants movie
        let similarParticipants = participants similar

        if movieParticipants.Count <> 0 then
            let sz = float movieParticipants.Count
            movieParticipants.IntersectWith(similarParticipants)
            let intersectSz = float movieParticipants.Count
            intersectSz / sz
        else
            0.0

    let fromTags =
        if movie.tags.Count <> 0 then
            let tags = HashSet(movie.tags)
            let sz = float tags.Count
            tags.IntersectWith(similar.tags)
            let intersectSz = float tags.Count
            intersectSz / sz
        else
            0.0

    let weighted =
        0.5 * fromRating
        + 0.25 * fromParticipants
        + 0.25 * fromTags

    min 1.0 weighted

let topNSimilarities (n: int) (confidence: MovieInfo -> MovieInfo -> float) (movie: MovieInfo) (similar: MovieInfo seq) =
    Array.ofSeq similar
    |> Array.sortBy (confidence movie)
    |> Seq.truncate n
    |> Seq.map (fun info -> {
        movieId = movie.id
        similarId = info.id
        confidence = confidence movie info
    })
