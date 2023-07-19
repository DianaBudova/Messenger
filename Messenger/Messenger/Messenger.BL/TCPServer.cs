using System.Text.Json;
using Messenger.Models.Application;
using SuperSimpleTcp;

namespace Messenger.BL;

public class TCPServer
{
    public event Action? ClientConnected;
    public event Action<string>? ClientDisconnected;
    public event Action<Message>? MessageReceived;
    private SimpleTcpServer server;
    public List<string> Clients { get; private set; }

    public TCPServer(string ipAddress, int port)
    {
        this.server = new(ipAddress, port);
        this.server.Events.ClientConnected += Events_ClientConnected;
        this.server.Events.ClientDisconnected += Events_ClientDisconnected;
        this.server.Events.DataReceived += Events_DataReceived;
        this.server.Start();
        this.Clients = new();
    }

    ~TCPServer()
    {
        this.server.Stop();
    }

    public void Start()
    {
        this.server.Start();
    }

    public void Stop()
    {
        this.server.Stop();
    }

    private void Events_ClientConnected(object? sender, ConnectionEventArgs e)
    {
        this.Clients.Add(e.IpPort);
        this.ClientConnected?.Invoke();
    }

    private void Events_ClientDisconnected(object? sender, ConnectionEventArgs e)
    {
        this.ClientDisconnected?.Invoke(e.IpPort);
    }

    private void Events_DataReceived(object? sender, SuperSimpleTcp.DataReceivedEventArgs e)
    {
        byte[] receivedData = e.Data.ToArray();
        Message receivedMessage = JsonSerializer.Deserialize<Message>(receivedData);
        this.MessageReceived?.Invoke(receivedMessage);
    }

    public void SendMessage(string ipPort, Message message)
    {
        byte[] data = JsonSerializer.SerializeToUtf8Bytes(message);
        if (this.server.IsConnected(ipPort))
            this.server.Send(ipPort, data);
    }
}
