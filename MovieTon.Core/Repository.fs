module MovieTon.Core.Repository

open MovieTon.Core.Movie
open MovieTon.Core.Staff
open MovieTon.Core.Tag

type MovieRepository = {
    getMovie: int -> Movie option
    getMovieByTitle: string -> Movie option
    getTitles: int -> Title seq option

    putMovies: Movie seq -> unit
    putTitles: Title seq -> unit
}

type StaffRepository = {
    getParticipationByName: string -> Participation seq option
    getDirectorActors: int -> StaffMember option * StaffMember seq option

    putStaffMembers: StaffMember seq -> unit
    putParticipation: Participation seq -> unit
}

type TagRepository = {
    getMoviesWithTagName: string -> MovieTag seq option
    getTagsByMovie: int -> Tag seq option

    putTags: Tag seq -> unit
    putMovieTags: MovieTag seq -> unit
}
