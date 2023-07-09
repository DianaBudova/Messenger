namespace Messenger.Models.Application;

public struct Message
{
    public User User { get; set; }
    public byte[] Content { get; set; }
    public MessageType Type { get; set; }
}
