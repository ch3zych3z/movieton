module MovieTon.Core.View

type StaffView = {
    name: string
}

type MovieView = {
    title: string
    director: StaffView option
    actors: StaffView Set
    tags: string Set
    rating: int
}

type MovieViewBuilder = {
    mutable title: string
    mutable director: StaffView option
    mutable actors: StaffView seq
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
        MovieView.title = x.title
        director = x.director
        actors = Set.ofSeq x.actors
        tags = Set.ofSeq x.tags
        rating = x.rating
    }
