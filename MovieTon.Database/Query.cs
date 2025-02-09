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
        builder.id = movie.Id;

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
            .Include(m => m.Titles)
            .FirstOrDefault();
    }

    public static Info.MovieInfo? GetMovieById(MovieDb db, int id, Func<IEnumerable<DbTitle>, string> titleSelector)
    {
        using var ctx = db.NewContext();
        var movie = GetMovieById(ctx, id);
        return movie == null ? null : BuildInfo(movie, titleSelector);
    }

    public static IEnumerable<Info.MovieInfo>? GetSimilar(MovieDb db, int size, int id, Func<IEnumerable<DbTitle>, string> titleSelector)
    {
        using var ctx = db.NewContext();
        var movie = ctx.Movies
            .Where(m => m.Id == id)
            .Include(m => m.SimilaritiesFrom)
            .Include(m => m.SimilarFrom)
            .FirstOrDefault();

        if (movie == null) return null;

        var similarities = movie.SimilaritiesFrom.ToArray();
        Array.Sort(similarities, (a, b) => (int)Math.Ceiling(10 * (b.Confidence - a.Confidence)));
        var similarMovies =
            similarities
                .Take(size)
                .Select(s => GetMovieById(ctx, s.SimilarToId))
                .OfType<DbMovie>()
                .Select(m => BuildInfo(m, titleSelector))
                .ToList();
        return similarMovies;
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
        return tag?.Movies.Where(m => m.Titles.Count != 0).Select(m => BuildInfo(m, titleSelector));
    }

    public static void AddMovies(MovieDb db, IEnumerable<DbMovie> movies)
    {
        using var ctx = db.NewContext();
        ctx.Movies.UpdateRange(movies);
        ctx.SaveChanges();
    }

    public static void AddTitles(MovieDb db, IEnumerable<DbTitle> titles)
    {
        using var ctx = db.NewContext();
        ctx.Titles.UpdateRange(titles);
        ctx.SaveChanges();
    }

    public static void AddStaffMembers(MovieDb db, IEnumerable<DbStaffMember> staffMembers)
    {
        using var ctx = db.NewContext();
        ctx.StaffMembers.UpdateRange(staffMembers);
        ctx.SaveChanges();
    }

    public static void AddParticipation(MovieDb db, IEnumerable<DbParticipation> participation)
    {
        using var ctx = db.NewContext();
        ctx.Participation.UpdateRange(participation);
        ctx.SaveChanges();
    }

    public static void AddTags(MovieDb db, IEnumerable<DbTag> tags)
    {
        using var ctx = db.NewContext();
        ctx.Tags.UpdateRange(tags);
        ctx.SaveChanges();
    }

    public static void AddMovieTags(MovieDb db, IEnumerable<DbMovieTag> movieTags)
    {
        using var ctx = db.NewContext();
        ctx.MovieTags.UpdateRange(movieTags);
        ctx.SaveChanges();
    }

    public static void AddSimilarities(MovieDb db, IEnumerable<DbSimilarity> similarities)
    {
        using var ctx = db.NewContext();
        ctx.Similarities.UpdateRange(similarities);
        ctx.SaveChanges();
    }
}
