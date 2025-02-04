module MovieTon.CLIRunner.Repository

open System.Collections.Generic

open MovieTon.Core.Tag
open MovieTon.Core.Movie
open MovieTon.Core.Staff
open MovieTon.Core.Info
open MovieTon.Utils.Collections
open MovieTon.Core.Repository

type private DictRepository = {
    movies: Dictionary<int, Movie>
    titles: Dictionary<string, Title>
    titlesById: Dictionary<int, List<Title>>

    staffMembers: Dictionary<int, StaffMember>
    participationByStaff: Dictionary<string, List<Participation>>
    participationByMovie: Dictionary<int, List<Participation>>

    name2Tag: Dictionary<string, Tag>
    tags: Dictionary<int, Tag>
    tagsByMovie: Dictionary<int, List<MovieTag>>
    moviesByTag: Dictionary<int, List<MovieTag>>
}
with
    static member Empty() = {
        movies = Dictionary()
        titles = Dictionary()
        titlesById = Dictionary()
        staffMembers = Dictionary()
        participationByStaff = Dictionary()
        participationByMovie = Dictionary()
        name2Tag = Dictionary()
        tags = Dictionary()
        tagsByMovie = Dictionary()
        moviesByTag = Dictionary()
    }

let private getMovie repo id =
    Dictionary.get id repo.movies

let private getMovieByTitle repo title =
    match Dictionary.get title repo.titles with
    | Some title ->
        Dictionary.get title.movieId repo.movies
    | None -> None

let private getTitles repo id =
    Dictionary.get id repo.titlesById
    |> Option.map Seq.cast<Title>

let private putMovies repo (movies: Movie seq) =
    for movie in movies do
        repo.movies.Add(movie.id, movie)

let private putTitles repo (titles: Title seq) =
    for title in titles do
        repo.titles[title.title] <- title

        let movieTitles = Dictionary.getOrUpdate title.movieId (List()) repo.titlesById
        movieTitles.Add(title)

let private getParticipationByName repo name =
    Dictionary.get name repo.participationByStaff
    |> Option.map Seq.cast<Participation>

let private staffByParticipation repo p =
    repo.staffMembers[p.staffId]
let private getDirectorActors repo movieId =
    match Dictionary.get movieId repo.participationByMovie with
    | Some participation ->
        let director, actors = participation |> List.ofSeq |> List.partition _.IsDirector
        let director =
            Seq.tryHead director
            |> Option.map (staffByParticipation repo)
        let actors = Seq.map (staffByParticipation repo) actors
        director, Some actors
    | None -> None, None

let private putStaffMembers repo members =
    for mem in members do
        repo.staffMembers[mem.id] <- mem

let private putParticipation repo participation =
    for p in participation do
        match Dictionary.get p.staffId repo.staffMembers with
        | Some staff ->
            let partByStaff = Dictionary.getOrUpdate staff.name (List()) repo.participationByStaff
            partByStaff.Add(p)

            let partByMovie = Dictionary.getOrUpdate p.movieId (List()) repo.participationByMovie
            partByMovie.Add(p)
        | None -> ()

let private getMoviesWithTagName repo name =
    match Dictionary.get name repo.name2Tag with
    | Some tag ->
        Dictionary.get tag.id repo.moviesByTag
        |> Option.map Seq.cast<MovieTag>
    | None -> None

let private getTagsByMovie repo movieId =
    Dictionary.get movieId repo.tagsByMovie
    |> Option.map (Seq.choose (fun mt -> Dictionary.get mt.tagId repo.tags))

let private putTags repo (tags: Tag seq) =
    for tag in tags do
        repo.tags[tag.id] <- tag
        repo.name2Tag[tag.name] <- tag

let private putMovieTags repo (movieTags: MovieTag seq) =
    for movieTag in movieTags do
        let tagsByMovie = Dictionary.getOrUpdate movieTag.movieId (List()) repo.tagsByMovie
        tagsByMovie.Add(movieTag)

        let moviesByTag = Dictionary.getOrUpdate movieTag.tagId (List()) repo.moviesByTag
        moviesByTag.Add(movieTag)

let private staffMemberView (sm: StaffMember) = { StaffInfo.name = sm.name }

let private movieView repo (title: string) (movie: Movie) =
    let movieBuilder = MovieInfoBuilder.Empty()

    movieBuilder.title <- title
    movieBuilder.rating <- movie.rating

    let director, actors = getDirectorActors repo movie.id
    match director with
    | Some director ->
        movieBuilder.director <- staffMemberView director |> Some
    | _ -> ()
    match actors with
    | Some actors ->
        movieBuilder.actors <- Seq.map staffMemberView actors

        match getTagsByMovie repo movie.id with
        | Some tags ->
            movieBuilder.tags <- tags |> Seq.map _.name
            movieBuilder.Build() |> Some
        | None -> None
    | None -> None

let private getMovieViewByTitle repo title =
    getMovieByTitle repo title
    |> Option.bind (movieView repo title)

let private movieViewById repo chooseTitle id =
    let movie = getMovie repo id
    match movie with
    | Some movie ->
        match getTitles repo id with
        | Some title ->
            let title = chooseTitle title
            movieView repo title movie
        | None -> None
    | None -> None

let private getStaffMovies repo name chooseTitle =
    let participation = getParticipationByName repo name
    match participation with
    | Some participation ->
        participation
        |> Seq.choose (fun p -> movieViewById repo chooseTitle p.movieId)
        |> Some
    | None -> None

let private getTagMovies repo tag chooseTitle =
    let movies = getMoviesWithTagName repo tag
    match movies with
    | Some movies ->
        movies
        |> Seq.choose (fun t -> movieViewById repo chooseTitle t.movieId)
        |> Some
    | None -> None

let empty () =
    let repo = DictRepository.Empty()
    {
        getMovieByTitle = getMovieViewByTitle repo
        putMovies = putMovies repo
        putTitles = putTitles repo

        getStaffMovies = getStaffMovies repo
        putStaffMembers = putStaffMembers repo
        putParticipation = putParticipation repo

        getTagMovies = getTagMovies repo
        putTags = putTags repo
        putMovieTags = putMovieTags repo
    }
