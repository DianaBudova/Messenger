namespace Messenger.Models.DB;

public class Endpoint
{
    public int Id { get; set; }
    public string IpAddress { get; set; } = null!;
    public string Port { get; set; } = null!;
    public List<User>? User { get; set; } = new();
}
