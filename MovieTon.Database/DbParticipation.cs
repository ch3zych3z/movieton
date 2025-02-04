using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MovieTon.Database;

public class DbParticipation(int staffId, int movieId, string role)
{
    public int StaffId { get; set; } = staffId;
    public DbStaffMember StaffMember { get; set; } = null!;

    public int MovieId { get; set; } = movieId;
    public DbMovie Movie { get; set; } = null!;

    public string? Role { get; set; } = role;
}
