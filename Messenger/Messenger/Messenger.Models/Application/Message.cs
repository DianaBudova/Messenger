using Messenger.Models.DB;

namespace Messenger.Models.Application;

public struct Message
{
    public User Sender { get; set; }
    public User Recipient { get; set; }
    public byte[] Content { get; set; } = new byte[0];
    public DateTime DateTime { get; set; }
    public MessageType Type { get; set; }

    public Message()
    {
    }

    public Message(Message message)
    {
        this.Sender = new(message.Sender);
        this.Recipient = new(message.Recipient);
        Array.Copy(message.Content, this.Content, message.Content.Length);
        this.DateTime = new(message.DateTime.Ticks);
        this.Type = message.Type;
    }
}
