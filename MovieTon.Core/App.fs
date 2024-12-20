module MovieTon.Core.App

open MovieTon.Core.Movie
open MovieTon.Core.Repository
open MovieTon.Core.Staff
open MovieTon.Core.Tag
open MovieTon.Core.View

type MovieTonApp(
    movieRepository: MovieRepository,
    staffRepository: StaffRepository,
    tagRepository: TagRepository,
    titleViewStrategy: Title seq -> string
) =

    let staffMemberView (sm: StaffMember) = { StaffView.name = sm.name }

    let movieView (title: string) (movie: Movie) =
        let movieBuilder = MovieViewBuilder.Empty()

        movieBuilder.title <- title
        movieBuilder.rating <- movie.rating

        let director, actors = staffRepository.getDirectorActors movie.id
        match director with
        | Some director ->
            movieBuilder.director <- staffMemberView director |> Some
        | _ -> ()
        match actors with
        | Some actors ->
            movieBuilder.actors <- Seq.map staffMemberView actors

            match tagRepository.getTagsByMovie movie.id with
            | Some tags ->
                movieBuilder.tags <- tags |> Seq.map _.name
                movieBuilder.Build() |> Some
            | None -> None
        | None -> None

    let movieViewByTitle title =
        movieRepository.getMovieByTitle title
        |> Option.bind (movieView title)

    let movieViewById id =
        let movie = movieRepository.getMovie id
        match movie with
        | Some movie ->
            match movieRepository.getTitles id with
            | Some title ->
                let title = title |> titleViewStrategy
                movieView title movie
            | None -> None
        | None -> None

    member x.GetMovieInfo(title: string) = movieViewByTitle title

    member x.GetStaffInfo(name: string) =
        let participation = staffRepository.getParticipationByName name
        match participation with
        | Some participation ->
            participation
            |> Seq.choose (fun p -> movieViewById p.movieId)
            |> Set.ofSeq
            |> Some
        | None -> None

    member x.GetTagMovies(tag: string) =
        let movies = tagRepository.getMoviesWithTagName tag
        match movies with
        | Some movies ->
            movies
            |> Seq.choose (fun t -> movieViewById t.movieId)
            |> Set.ofSeq
            |> Some
        | None -> None

    member x.PutStaffMembers (staff: StaffMember seq) = staffRepository.putStaffMembers staff
    member x.PutParticipation (participation: Participation seq) = staffRepository.putParticipation participation
    member x.PutMovies (movies: Movie seq) = movieRepository.putMovies movies
    member x.PutTitles (titles: Title seq) = movieRepository.putTitles titles
    member x.PutTags (tags: Tag seq) = tagRepository.putTags tags
    member x.PutMovieTags (movieTags: MovieTag seq) = tagRepository.putMovieTags movieTags
