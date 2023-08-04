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
        IpAddress = "192.168.77.166",
        Port = 8888,
    };

    public override bool Equals(object? another)
    {
        if (another is null || another is not Server)
            return false;
        Server server = (Server)another;
        return this.NameServer == server.NameServer
            && this.IpAddress == server.IpAddress
            && this.Port == server.Port;
    }
}
