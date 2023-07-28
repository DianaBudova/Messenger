using Messenger.Models.DB;

namespace Messenger.Repositories.Interfaces;

public interface IChatRepository
{
    public Chat? Add(Chat chat);
    public Chat? Update(Chat chat);
    public bool Remove(Chat chat);
    public bool Exists(Chat chat);
    public Chat? GetById(int id);
    public List<Chat>? GetBySenderId(int id);
    public List<Chat>? GetByRecipientId(int id);
    public List<Chat>? GetBySenderRecipientId(int senderId, int recipientId);
    public List<Chat>? GetAll();
}
