using System.Configuration;

namespace Messenger.Models.DB;

public class Server
{
    public int Id { get; set; }
    public string NameServer { get; set; }
    public string IpAddress { get; set; }
    public int Port { get; set; }

    public static Server DefaultServer { get; } = new()
    {
        Id = 0,
        NameServer = ConfigurationManager.AppSettings["ServerNameByDefault"]!,
        IpAddress = "127.0.0.1",
        Port = 8888,
    };
}
