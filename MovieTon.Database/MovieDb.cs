using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MovieTon.Database;

public sealed class MovieDbContext(string connectionString) : DbContext
{
    public DbSet<DbMovie> Movies { get; set; }
    public DbSet<DbTitle> Titles { get; set; }
    public DbSet<DbParticipation> Participation { get; set; }
    public DbSet<DbStaffMember> StaffMembers { get; set; }
    public DbSet<DbTag> Tags { get; set; }
    public DbSet<DbMovieTag> MovieTags { get; set; }

    public DbSet<DbSimilarity> Similarities { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql(connectionString,
            ServerVersion.AutoDetect(connectionString));
        optionsBuilder.EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .ApplyConfiguration(new MovieConfiguration())
            .ApplyConfiguration(new TitleConfiguration());
    }
}

public sealed class MovieDb(string defaultConnectionString)
{
    public MovieDbContext NewContext() => new MovieDbContext(defaultConnectionString);
}
