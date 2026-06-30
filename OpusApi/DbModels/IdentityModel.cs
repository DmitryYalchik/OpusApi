using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpusApi.DbModels;

public abstract class IdentityModel
{
    [Column("id")] [Key]
    public Guid Id { get; set; } = Guid.CreateVersion7();
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public abstract bool Validate(out string? errorMessage);
}