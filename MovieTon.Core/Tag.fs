module MovieTon.Core.Tag

type Tag = {
    id: int
    name: string
}
with
    static member Of id name = {
        id = id
        name = name
    }

type MovieTag = {
    tagId: int
    movieId: int
}
with
    static member Of tagId movieId = {
        tagId = tagId
        movieId = movieId
    }
