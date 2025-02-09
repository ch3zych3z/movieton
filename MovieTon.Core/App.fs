module MovieTon.Core.App

open MovieTon.Core.Movie
open MovieTon.Core.Repository
open MovieTon.Core.Similarity
open MovieTon.Core.Staff
open MovieTon.Core.Tag

type MovieTonApp(
    repo: Repository,
    titleViewStrategy: Title seq -> string
) =
    member x.GetMovieInfo(title: string) = repo.getMovieByTitle title
    member x.GetStaffInfo(name: string) = repo.getStaffMovies name titleViewStrategy
    member x.GetTagMovies(tag: string) = repo.getTagMovies tag titleViewStrategy
    member x.GetSimilar(count: int, id: int) = repo.getSimilar count id titleViewStrategy

    member x.PutStaffMembers (staff: StaffMember seq) = repo.putStaffMembers staff
    member x.PutParticipation (participation: Participation seq) = repo.putParticipation participation
    member x.PutMovies (movies: Movie seq) = repo.putMovies movies
    member x.PutTitles (titles: Title seq) = repo.putTitles titles
    member x.PutTags (tags: Tag seq) = repo.putTags tags
    member x.PutMovieTags (movieTags: MovieTag seq) = repo.putMovieTags movieTags
    member x.PutSimilarities (similarities: Similarity seq) = repo.putSimilarities similarities
