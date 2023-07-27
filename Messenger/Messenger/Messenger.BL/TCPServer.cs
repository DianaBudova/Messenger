using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using Messenger.Models.DB;
using Messenger.Repositories;
using SimpleTCP;

namespace Messenger.BL;

public class TCPServer
{
    public event Action? ClientsChanged;
    public event Action<bool>? StateChanged;
    public event Action<Models.Application.Message>? MessageReceived;
    private readonly SimpleTcpServer server;
    private readonly IPEndPoint ep;
    private bool isStarted;
    public bool IsStarted
    {
        get
        { return this.isStarted; }
        private set
        {
            this.isStarted = value;
            this.StateChanged?.Invoke(this.isStarted);
        }
    }
    public List<TcpClient> Clients { get; private set; }
    public List<Models.Application.Message> Messages { get; private set; }

    public TCPServer(IPEndPoint ep)
    {
        this.ep = ep;
        this.server = new();
        this.server.ClientConnected += Server_ClientConnected;
        this.server.ClientDisconnected += Server_ClientDisconnected;
        this.server.DataReceived += Server_DataReceived;
        this.Clients = new();
        this.Messages = new();
        this.IsStarted = this.server.IsStarted;
    }

    public void Start()
    {
        if (!this.server.IsStarted)
            this.server.Start(this.ep.Address, this.ep.Port);
        this.IsStarted = this.server.IsStarted;
    }

    public void Stop()
    {
        if (this.server.IsStarted)
        {
            Models.Application.Message serverStoppedMessage = new() { Type = Models.Application.MessageType.EndOfLine };
            byte[] serializedMessage = JsonSerializer.SerializeToUtf8Bytes(serverStoppedMessage);
            this.server.Broadcast(serializedMessage);
            this.server.Stop();
            this.Clients.ForEach(client => client.Client.Dispose());
            this.Clients.ForEach(client => client.Client.Close());
            this.Clients.Clear();
            this.ClientsChanged?.Invoke();
        }
        this.IsStarted = this.server.IsStarted;
    }

    public void SendMessage(TcpClient tcpClient, Models.Application.Message message)
    {
        byte[] data = JsonSerializer.SerializeToUtf8Bytes(message);
        if (this.server.IsStarted)
        {
            try
            { tcpClient.GetStream().Write(data, 0, data.Length); }
            catch
            { throw new Exception("Error occurred while sending a message"); }
        }
    }

    private void Server_ClientConnected(object? sender, TcpClient e)
    {
        this.Clients.Add(e);
        this.ClientsChanged?.Invoke();
    }

    private void Server_ClientDisconnected(object? sender, TcpClient e)
    {
        this.Clients.Remove(e);
        this.ClientsChanged?.Invoke();
    }

    private void Server_DataReceived(object? sender, Message e)
    {
        byte[] receivedData = e.Data.ToArray();
        Models.Application.Message receivedMessage = JsonSerializer.Deserialize<Models.Application.Message>(receivedData);
        if (receivedMessage.Sender is null
            || receivedMessage.Recipient is null
            || receivedMessage.Content is null 
            || receivedMessage.Content.Length <= 0
            || receivedMessage.Type == Models.Application.MessageType.EndOfLine)
            return;
        this.Messages.Add(receivedMessage);
        this.MessageReceived?.Invoke(receivedMessage);
        Chat chat = new()
        {
            SenderId = RepositoryFactory.GetUserRepository().GetByNickname(receivedMessage.Sender.Nickname).Id,
            RecipientId = RepositoryFactory.GetUserRepository().GetByNickname(receivedMessage.Recipient.Nickname).Id,
            Message = receivedMessage.Content,
            MessageType = receivedMessage.Type,
            DateTime = receivedMessage.DateTime,
        };
        RepositoryFactory.GetChatRepository().Add(chat);
    }
}
