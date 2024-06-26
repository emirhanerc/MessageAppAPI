using MessageAppAPI.Context;
using MessageAppAPI.Entities;

namespace MessageAppAPI.Repositories;

public class MessageRepository : BaseRepository<Message>, IMessageRepository
{
    public MessageRepository(ApplicationDbContext context) : base(context)
    {
    }
}