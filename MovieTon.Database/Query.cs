using Microsoft.EntityFrameworkCore;
using MovieTon.Core;

namespace MovieTon.Database;

public static class Query
{

    private static Info.MovieInfo BuildInfo(string title, DbMovie movie)
    {
        var builder = Info.MovieInfoBuilder.Empty();
        builder.title = title;
        builder.rating = movie.Rating;
        builder.tags = movie.Tags.Select(t => t.Name);

        var directorIds = movie.Participation
            .Where(p => p.Role == "director")
            .Select(p => p.StaffId)
            .ToHashSet();
        var director = movie.StaffMembers
            .Where(sm => directorIds.Contains(sm.Id))
            .Select(sm => sm.Name)
            .FirstOrDefault();
        var actors = movie.StaffMembers
            .Where(sm => !directorIds.Contains(sm.Id))
            .Select(sm => sm.Name);

        if (director != null)
        {
            builder.director = new Info.StaffInfo(director);
        }

        builder.actors = actors.Select(name => new Info.StaffInfo(name));
        return builder.Build();
    }

    private static Info.MovieInfo BuildInfo(DbMovie movie, Func<IEnumerable<DbTitle>, string> titleSelector)
    {
        var title = titleSelector(movie.Titles);
        return BuildInfo(title, movie);
    }

    private static DbMovie? GetMovieById(MovieDbContext ctx, int id)
    {
        return ctx.Movies
            .Where(m => m.Id == id)
            .Include(m => m.Tags)
            .Include(m => m.Participation)
            .Include(m => m.StaffMembers)
            .FirstOrDefault();
    }

    public static Info.MovieInfo? GetMovieByTitle(MovieDb db, string title)
    {
        using var ctx = db.NewContext();
        var dbTitle = ctx.Titles.FirstOrDefault(t => t.Title == title);
        if (dbTitle == null) return null;

        var movie = GetMovieById(ctx, dbTitle.MovieId);
        return movie == null ? null : BuildInfo(title, movie);
    }

    public static IEnumerable<Info.MovieInfo>? GetStaffMovies(MovieDb db, string name,
        Func<IEnumerable<DbTitle>, string> titleSelector)
    {
        using var ctx = db.NewContext();
        var staffMember = ctx.StaffMembers
            .Where(sm => sm.Name == name)
            .Include(sm => sm.Movies).ThenInclude(m => m.Tags)
            .Include(sm => sm.Movies).ThenInclude(m => m.Participation)
            .Include(sm => sm.Movies).ThenInclude(m => m.StaffMembers)
            .Include(sm => sm.Movies).ThenInclude(m => m.Titles)
            .FirstOrDefault();
        return staffMember?.Movies.Select(m => BuildInfo(m, titleSelector));
    }

    public static IEnumerable<Info.MovieInfo>? GetTagMovies(MovieDb db, string name,
        Func<IEnumerable<DbTitle>, string> titleSelector)
    {
        using var ctx = db.NewContext();
        var tag = ctx.Tags
            .Where(sm => sm.Name == name)
            .Include(t => t.Movies).ThenInclude(m => m.Tags)
            .Include(t => t.Movies).ThenInclude(m => m.Participation)
            .Include(t => t.Movies).ThenInclude(m => m.StaffMembers)
            .Include(t => t.Movies).ThenInclude(m => m.Titles)
            .FirstOrDefault();
        return tag?.Movies.Select(m => BuildInfo(m, titleSelector));
    }

    public static void AddMovies(MovieDb db, IEnumerable<DbMovie> movies)
    {
        using var ctx = db.NewContext();
        ctx.Movies.AddRange(movies);
        ctx.SaveChanges();
    }

    public static void AddTitles(MovieDb db, IEnumerable<DbTitle> titles)
    {
        using var ctx = db.NewContext();
        ctx.Titles.AddRange(titles);
        ctx.SaveChanges();
    }

    public static void AddStaffMembers(MovieDb db, IEnumerable<DbStaffMember> staffMembers)
    {
        using var ctx = db.NewContext();
        ctx.StaffMembers.AddRange(staffMembers);
        ctx.SaveChanges();
    }

    public static void AddParticipation(MovieDb db, IEnumerable<DbParticipation> participation)
    {
        using var ctx = db.NewContext();
        ctx.Participation.AddRange(participation);
        ctx.SaveChanges();
    }

    public static void AddTags(MovieDb db, IEnumerable<DbTag> tags)
    {
        using var ctx = db.NewContext();
        ctx.Tags.AddRange(tags);
        ctx.SaveChanges();
    }

    public static void AddMovieTags(MovieDb db, IEnumerable<DbMovieTag> movieTags)
    {
        using var ctx = db.NewContext();
        ctx.MovieTags.AddRange(movieTags);
        ctx.SaveChanges();
    }
}
