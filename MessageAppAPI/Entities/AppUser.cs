using Microsoft.AspNetCore.Identity;

namespace MessageAppAPI.Entities;

public class AppUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Role { get; set; }
    public ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();
    public ICollection<Message> SentMessages { get; set; } = new List<Message>();
}