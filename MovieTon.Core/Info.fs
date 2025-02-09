module MovieTon.Core.Info

open System.Collections.Generic

type StaffInfo = {
    name: string
}

[<CustomEquality; NoComparison>]
type MovieInfo = {
    id: int
    title: string
    director: StaffInfo option
    actors: HashSet<StaffInfo>
    tags: HashSet<string>
    rating: int
}
with
    override x.GetHashCode() = x.id

    override x.Equals(obj) =
        match obj with
        | :? MovieInfo as mf -> x.id = mf.id
        | _ -> false

type MovieInfoBuilder = {
    mutable id: int
    mutable title: string
    mutable director: StaffInfo option
    mutable actors: StaffInfo seq
    mutable tags: string seq
    mutable rating: int
}
with
    static member Empty() = {
        id = -1
        title = ""
        director = None
        actors = []
        tags = []
        rating = -1
    }

    member x.Build() = {
        MovieInfo.id = x.id
        title = x.title
        director = x.director
        actors = HashSet(x.actors)
        tags = HashSet(x.tags)
        rating = x.rating
    }
