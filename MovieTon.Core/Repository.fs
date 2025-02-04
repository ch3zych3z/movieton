module MovieTon.Core.Repository

open MovieTon.Core.Movie
open MovieTon.Core.Staff
open MovieTon.Core.Tag
open MovieTon.Core.Info

type Repository = {
    getMovieByTitle: string -> MovieInfo option
    putMovies: Movie seq -> unit
    putTitles: Title seq -> unit

    getStaffMovies: string -> (Title seq -> string) -> MovieInfo seq option
    putStaffMembers: StaffMember seq -> unit
    putParticipation: Participation seq -> unit

    getTagMovies: string -> (Title seq -> string) -> MovieInfo seq option
    putTags: Tag seq -> unit
    putMovieTags: MovieTag seq -> unit
}
