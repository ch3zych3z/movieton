using System.ComponentModel.DataAnnotations;

namespace MovieTon.Database;

public class DbStaffMember(int id, string name)
{
    public int Id { get; set; } = id;
    [Required] public string? Name { get; set; } = name;

    public ICollection<DbParticipation> Participation { get; set; } = null!;
    public ICollection<DbMovie> Movies { get; set; } = null!;
}
