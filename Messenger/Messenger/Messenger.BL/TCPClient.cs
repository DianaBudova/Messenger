using SimpleTCP;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;

namespace Messenger.BL;

public class TCPClient
{
    public event Action<Models.Application.Message>? MessageReceived;
    private readonly IPEndPoint ep;
    private readonly SimpleTcpClient client;

    public TCPClient(IPEndPoint ep)
    {
        this.ep = ep;
        this.client = new();
        this.client.DataReceived += Events_DataReceived;
    }

    public TcpClient Connect()
    {
        return this.client.Connect(ep.Address.ToString(), ep.Port).TcpClient ??
            throw new SocketException();
    }

    public void Disconnect()
    {
        this.client.Disconnect();
    }

    private void Events_DataReceived(object? sender, SimpleTCP.Message e)
    {
        byte[] receivedData = e.Data.ToArray();
        Models.Application.Message receivedMessage = JsonSerializer.Deserialize<Models.Application.Message>(receivedData);
        this.MessageReceived?.Invoke(receivedMessage);
    }

    public void SendMessage(Models.Application.Message message)
    {
        byte[] data = JsonSerializer.SerializeToUtf8Bytes(message);
        this.client.Write(data);
    }
}
