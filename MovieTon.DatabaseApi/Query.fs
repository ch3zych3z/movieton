module internal MovieTon.Database.Api.Query

open MovieTon.Core.Movie
open MovieTon.Core.Staff
open MovieTon.Core.Tag
open MovieTon.Database

let private toOption v =
    if obj.ReferenceEquals(v, null) then None
    else Some v

let getMovieByTitle db title =
    Query.GetMovieByTitle(db, title)
    |> toOption

let putMovies db (movies: Movie seq) =
    let dbMovies = movies |> Seq.map (fun m -> DbMovie(m.id, m.rating))
    Query.AddMovies(db, dbMovies)

let putTitles db (titles: Title seq) =
    let dbTitles = titles |> Seq.map (fun t -> DbTitle(t.title, t.local.ToString(), t.movieId))
    Query.AddTitles(db, dbTitles)

let private fromDbTitle (t: DbTitle) = { title = t.Title; local = Localization.UnsafeParse(t.Local); movieId = t.MovieId }

let getStaffMovies db name titleSelector =
    Query.GetStaffMovies(db, name, Seq.map fromDbTitle >> titleSelector)
    |> toOption

let putStaffMembers db (staffMembers: StaffMember seq) =
    let dbStaffMembers = staffMembers |> Seq.map (fun sm -> DbStaffMember(sm.id, sm.name))
    Query.AddStaffMembers(db, dbStaffMembers)

let putParticipation db (participation: Participation seq) =
    let dbParticipation = participation |> Seq.map (fun p -> DbParticipation(p.staffId, p.movieId, p.role.ToString()))
    Query.AddParticipation(db, dbParticipation)

let getTagMovies db tag titleSelector =
    Query.GetTagMovies(db, tag, Seq.map fromDbTitle >> titleSelector)
    |> toOption

let putTags db (tags: Tag seq) =
    let dbTags = tags |> Seq.map (fun t -> DbTag(t.id, t.name))
    Query.AddTags(db, dbTags)

let putMovieTags db (movieTags: MovieTag seq) =
    let dbMovieTags = movieTags |> Seq.map (fun mt -> DbMovieTag(mt.tagId, mt.movieId))
    Query.AddMovieTags(db, dbMovieTags)
