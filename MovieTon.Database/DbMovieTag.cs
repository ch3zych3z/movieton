namespace MovieTon.Database;

public class DbMovieTag(int tagId, int movieId)
{
    public int TagId { get; set; } = tagId;
    public DbTag Tag { get; set; } = null!;

    public int MovieId { get; set; } = movieId;
    public DbMovie Movie { get; set; } = null!;
}
