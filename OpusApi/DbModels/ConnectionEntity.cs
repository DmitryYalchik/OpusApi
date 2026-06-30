using System.ComponentModel.DataAnnotations;
using System.Text;

namespace OpusApi.DbModels;

public class ConnectionEntity : IdentityModel
{
    public List<string> ConnectionId { get; set; } = [];
    public string? UserName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public override bool Validate(out string? errorMessage)
    {
        errorMessage = null;
        return true;
    }
}