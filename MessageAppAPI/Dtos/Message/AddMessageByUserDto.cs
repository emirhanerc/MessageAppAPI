namespace MessageAppAPI.Dtos.Message;

public class AddMessageByUserDto
{
    public string ReceiverUserId { get; set; }
    public string MessageContent { get; set; }
}