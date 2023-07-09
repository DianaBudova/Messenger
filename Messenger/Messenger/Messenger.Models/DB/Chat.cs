using Messenger.Models.Application;

namespace Messenger.Models.DB;

public class Chat
{
    public int Id { get; set; }
    public User User { get; set; }
    public string Message { get; set; } = null!;
    public MessageType MessageType { get; set; }
}
