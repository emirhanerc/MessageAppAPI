namespace MessageAppAPI.Entities;

public class Message : BaseEntity
{
    public string SenderId { get; set; }
    public AppUser Sender { get; set; }

    public string ReceiverId { get; set; }
    public AppUser Receiver { get; set; }

    public string Description { get; set; }
}