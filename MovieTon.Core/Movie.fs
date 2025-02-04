module MovieTon.Core.Movie

open System

type Movie = {
    id: int
    rating: int
}
with
    static member Of id rating = {
        id = id
        rating = rating
    }

type Localization =
    | Ru
    | En
with
    static member UnsafeParse(str: string) =
        if "ru".Equals(str, StringComparison.InvariantCultureIgnoreCase) then
            Ru
        else En

    override x.ToString() =
        match x with
        | Ru -> "ru"
        | En -> "en"

type Title = {
    title: string
    local: Localization
    movieId: int
}
with
    static member Of title local movieId = {
        title = title
        local = local
        movieId = movieId
    }
