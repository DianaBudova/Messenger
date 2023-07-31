using Messenger.Models.Application;

namespace Messenger.Models.DB;

public class Chat
{
    public int Id { get; set; }
    public int SenderId { get; set; }
    public int RecipientId { get; set; }
    public virtual User Sender { get; set; }
    public virtual User Recipient { get; set; }
    public byte[] Message { get; set; }
    public MessageType MessageType { get; set; }
    public DateTime DateTime { get; set; }

    public bool IsSimilar(Chat? another)
    {
        if (another is null)
            return false;
        if (this.SenderId == another.SenderId)
            if (this.RecipientId == another.RecipientId)
                if (this.MessageType == another.MessageType)
                    if (this.Message.SequenceEqual(another.Message))
                        if (this.DateTime == another.DateTime)
                            return true;
        return false;
    }

    public override bool Equals(object? another)
    {
        if (another is null || another is not Chat)
            return false;
        Chat chat = (Chat)another;
        return this.Id == chat.Id
            && this.SenderId == chat.SenderId
            && this.RecipientId == chat.RecipientId
            && this.MessageType == chat.MessageType
            && this.Message.SequenceEqual(chat.Message)
            && this.DateTime == chat.DateTime;
    }
}
