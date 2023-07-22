using Messenger.Models.Application;
using SuperSimpleTcp;
using System.Text.Json;

namespace Messenger.BL;

public class TCPClient
{
    public event Action<Message>? MessageReceived;
    private SimpleTcpClient client;

    public TCPClient(string ipAddress, int port)
    {
        this.client = new(ipAddress, 8888);
        this.client.Events.DataReceived += Events_DataReceived;
    }

    public void Connect()
    {
        this.client.Connect();
    }

    public void Disconnect()
    {
        this.client.Disconnect();
    }

    private void Events_DataReceived(object? sender, SuperSimpleTcp.DataReceivedEventArgs e)
    {
        byte[] receivedData = e.Data.ToArray();
        Message receivedMessage = JsonSerializer.Deserialize<Message>(receivedData);
        this.MessageReceived?.Invoke(receivedMessage);
    }

    public void SendMessage(Message message)
    {
        byte[] data = JsonSerializer.SerializeToUtf8Bytes(message);
        this.client.Send(data);
    }
}
