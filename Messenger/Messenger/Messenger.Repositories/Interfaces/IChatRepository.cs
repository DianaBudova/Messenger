using Messenger.Models.DB;

namespace Messenger.Repositories.Interfaces;

public interface IChatRepository
{
    public Chat? Add(Chat server);
    public Chat? Update(Chat server);
    public bool Remove(Chat server);
    public Chat? GetById(int id);
    public List<Chat>? GetBySenderId(int id);
    public List<Chat>? GetByRecipientId(int id);
    public List<Chat>? GetBySenderRecipientId(int senderId, int recipientId);
    public List<Chat>? GetAll();
}
