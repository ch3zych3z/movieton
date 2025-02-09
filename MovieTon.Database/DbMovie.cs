using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MovieTon.Database;

public class MovieConfiguration : IEntityTypeConfiguration<DbMovie>
{
    public void Configure(EntityTypeBuilder<DbMovie> builder)
    {
        builder
            .HasMany(m => m.Tags)
            .WithMany(t => t.Movies)
            .UsingEntity<DbMovieTag>(
                b => b
                    .HasOne(mt => mt.Tag)
                    .WithMany(t => t.MovieTags)
                    .HasForeignKey(mt => mt.TagId),
                b => b
                    .HasOne(mt => mt.Movie)
                    .WithMany(m => m.MovieTags)
                    .HasForeignKey(mt => mt.MovieId),
                b => b.HasKey(mt => new { mt.TagId, mt.MovieId })
            );

        builder
            .HasMany(m => m.StaffMembers)
            .WithMany(sm => sm.Movies)
            .UsingEntity<DbParticipation>(
                b => b
                    .HasOne(p => p.StaffMember)
                    .WithMany(sm => sm.Participation)
                    .HasForeignKey(p => p.StaffId),
                b => b
                    .HasOne(p => p.Movie)
                    .WithMany(m => m.Participation)
                    .HasForeignKey(p => p.MovieId),
                b =>
                {
                    b.HasKey(p => new { p.Role, p.StaffId, p.MovieId });
                    b
                        .Property(p => p.Role)
                        .IsRequired();
                }
            );

        builder
            .HasMany(m => m.SimilarFrom)
            .WithMany(m => m.SimilarTo)
            .UsingEntity<DbSimilarity>(
                b => b
                    .HasOne(s => s.SimilarFrom)
                    .WithMany(m => m.SimilaritiesFrom)
                    .HasForeignKey(s => s.SimilarFromId),
                b => b
                    .HasOne(s => s.SimilarTo)
                    .WithMany(m => m.SimilaritiesTo)
                    .HasForeignKey(s => s.SimilarToId)
            );
    }
}

public class DbMovie(int id, int rating)
{
    public int Id { get; set; } = id;
    public int Rating { get; set; } = rating;

    public ICollection<DbTitle> Titles { get; set; } = null!;

    public ICollection<DbMovieTag> MovieTags { get; set; } = null!;
    public ICollection<DbTag> Tags { get; set; } = null!;

    public ICollection<DbParticipation> Participation { get; set; } = null!;
    public ICollection<DbStaffMember> StaffMembers { get; set; } = null!;

    public ICollection<DbSimilarity> SimilaritiesFrom { get; set; } = null!;
    public ICollection<DbMovie> SimilarFrom { get; set; } = null!;

    public ICollection<DbSimilarity> SimilaritiesTo { get; set; } = null!;
    public ICollection<DbMovie> SimilarTo { get; set; } = null!;
}
