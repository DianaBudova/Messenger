using Messenger.DAL;
using Messenger.Models.DB;
using Messenger.Repositories.Interfaces;

namespace Messenger.Repositories;

internal class ChatRepository : IChatRepository
{
    private readonly DataContext context;

    public ChatRepository(DataContext context) =>
        this.context = context;

    public Chat? Add(Chat chat)
    {
        var result = this.context.Add(chat).Entity;
        try
        {
            this.context.SaveChanges();
            return result;
        }
        catch
        { return null; }
    }

    public Chat? Update(Chat chat)
    {
        var result = this.context.Update(chat).Entity;
        try
        {
            this.context.SaveChanges();
            return result;
        }
        catch
        { return null; }
    }

    public bool Remove(Chat chat)
    {
        this.context.Remove(chat);
        this.context.ReseedIdentity<Chat>();
        try
        { return this.context.SaveChanges() > 0; }
        catch
        { return false; }
    }

    public Chat? GetById(int id)
    {
        try
        {
            return this.context.Chat
                .Where(prop => prop.Id == id)
                .First();
        }
        catch
        { return null; }
    }

    public List<Chat>? GetBySenderId(int id)
    {
        try
        {
            return this.context.Chat
                .Where(prop => prop.SenderId == id)
                .ToList();
        }
        catch
        { return null; }
    }

    public List<Chat>? GetByRecipientId(int id)
    {
        try
        {
            return this.context.Chat
                .Where(prop => prop.RecipientId == id)
                .ToList();
        }
        catch
        { return null; }
    }

    public List<Chat>? GetBySenderRecipientId(int senderId, int recipientId)
    {
        try
        {
            return this.context.Chat
                .Where(prop => prop.SenderId == senderId)
                .Where(prop => prop.RecipientId == recipientId)
                .ToList();
        }
        catch
        { return null; }
    }

    public List<Chat>? GetAll()
    {
        try
        {
            return this.context.Chat.ToList();
        }
        catch
        { return null; }
    }
}
