using Messenger.Models.Application;
using Messenger.Models.DB;
using Messenger.Repositories;
using SuperSimpleTcp;
using System.Net.Sockets;
using System.Text.Json;

namespace Messenger.BL;

public class TCPClient
{
    public event Action<Message>? MessageReceived;
    private readonly SimpleTcpClient client;

    public TCPClient(string serverName)
    {
        Server? chosenServer = RepositoryFactory.GetServerRepository().GetByNameServer(serverName) 
            ?? throw new SocketException();
        this.client = new(chosenServer.IpAddress, chosenServer.Port);
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

    private void Events_DataReceived(object? sender, DataReceivedEventArgs e)
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
