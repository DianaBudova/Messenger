using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Net.Sockets;
using System.Text.Json;
using Messenger.Models.Application;
using Messenger.Models.DB;
using Messenger.Repositories;
using SuperSimpleTcp;

namespace Messenger.BL;

public class TCPServer
{
    public event Action<object?, NotifyCollectionChangedEventArgs>? ClientsChanged;
    public event Action<Message>? MessageReceived;
    private readonly SimpleTcpServer server;
    public ObservableCollection<string> Clients { get; private set; }

    public TCPServer(string serverName)
    {
        Server? chosenServer = RepositoryFactory.GetServerRepository().GetByNameServer(serverName)
            ?? throw new SocketException();
        this.server = new(chosenServer.IpAddress, chosenServer.Port);
        this.server.Events.ClientConnected += Events_ClientConnected;
        this.server.Events.ClientDisconnected += Events_ClientDisconnected;
        this.server.Events.DataReceived += Events_DataReceived;
        this.Clients = new();
        this.Clients.CollectionChanged += Clients_CollectionChanged;
    }

    private void Clients_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        this.ClientsChanged?.Invoke(sender, e);
    }

    public void Start()
    {
        if (!this.server.IsListening)
            this.server.Start();
    }

    public void Stop()
    {
        if (this.server.IsListening)
            this.server.Stop();
    }

    private void Events_ClientConnected(object? sender, ConnectionEventArgs e)
    {
        this.Clients.Add(e.IpPort);
    }

    private void Events_ClientDisconnected(object? sender, ConnectionEventArgs e)
    {
        this.Clients.Remove(e.IpPort);
    }

    private void Events_DataReceived(object? sender, DataReceivedEventArgs e)
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
