namespace Messenger.Models.Application;

public class Chat
{
    public List<Message> IncomingMessages { get; set; } = new();
    public List<Message> OutcomingMessages { get; set; } = new();
}
