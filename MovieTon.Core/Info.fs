module MovieTon.Core.Info

type StaffInfo = {
    name: string
}

type MovieInfo = {
    title: string
    director: StaffInfo option
    actors: StaffInfo Set
    tags: string Set
    rating: int
}

type MovieInfoBuilder = {
    mutable title: string
    mutable director: StaffInfo option
    mutable actors: StaffInfo seq
    mutable tags: string seq
    mutable rating: int
}
with
    static member Empty() = {
        title = ""
        director = None
        actors = []
        tags = []
        rating = -1
    }

    member x.Build() = {
        MovieInfo.title = x.title
        director = x.director
        actors = Set.ofSeq x.actors
        tags = Set.ofSeq x.tags
        rating = x.rating
    }
