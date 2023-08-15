using Messenger.Models.DB;

namespace Messenger.Models.Application;

public struct Message
{
    public User? Sender { get; set; }
    public User? Recipient { get; set; }
    public byte[]? Content { get; set; } = Array.Empty<byte>();
    public DateTime DateTime { get; set; }
    public MessageType Type { get; set; }

    public Message() { }
}
