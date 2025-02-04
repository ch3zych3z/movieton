module MovieTon.Database.Api.Repository

open MovieTon.Database
open MovieTon.Database.Api.Query
open MovieTon.Core.Repository

let make connectionString =
    let db = MovieDb(connectionString)
    {
        getMovieByTitle = getMovieByTitle db
        putMovies = putMovies db
        putTitles = putTitles db

        getStaffMovies = getStaffMovies db
        putStaffMembers = putStaffMembers db
        putParticipation = putParticipation db

        getTagMovies = getTagMovies db
        putTags = putTags db
        putMovieTags = putMovieTags db
    }
