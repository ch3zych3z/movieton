using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MovieTon.Database;

public class TitleConfiguration : IEntityTypeConfiguration<DbTitle>
{
    public void Configure(EntityTypeBuilder<DbTitle> builder)
    {
        builder.HasKey(t => new { t.Title, t.MovieId, t.Local });

        builder.Property(t => t.Title)
            .IsRequired();

        builder.Property(t => t.Title)
            .HasCharSet("utf8");

        builder.Property(t => t.Local)
            .IsRequired()
            .HasMaxLength(2);
    }
}

public class DbTitle(string title, string local, int movieId)
{
    public string Title { get; set; } = title;
    public string Local { get; set; } = local;
    public int MovieId { get; set; } = movieId;

    public DbMovie Movie { get; set; } = null!;
}
