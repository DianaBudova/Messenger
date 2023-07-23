using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using SimpleTCP;

namespace Messenger.BL;

public class TCPServer
{
    public event Action<object?, NotifyCollectionChangedEventArgs>? ClientsChanged;
    public event Action<Models.Application.Message>? MessageReceived;
    private readonly SimpleTcpServer server;
    private readonly IPEndPoint ep;
    public ObservableCollection<TcpClient> Clients { get; private set; }

    public TCPServer(IPEndPoint ep)
    {
        this.ep = ep;
        this.server = new();
        this.server.ClientConnected += Server_ClientConnected;
        this.server.ClientDisconnected += Server_ClientDisconnected;
        this.server.DataReceived += Server_DataReceived;
        this.Clients = new();
        this.Clients.CollectionChanged += Clients_CollectionChanged;
    }

    private void Server_ClientConnected(object? sender, TcpClient e)
    {
        this.Clients.Add(e);
    }

    private void Server_ClientDisconnected(object? sender, TcpClient e)
    {
        this.Clients.Remove(e);
    }

    private void Server_DataReceived(object? sender, Message e)
    {
        byte[] receivedData = e.Data.ToArray();
        Models.Application.Message receivedMessage = JsonSerializer.Deserialize<Models.Application.Message>(receivedData);
        this.MessageReceived?.Invoke(receivedMessage);
        this.SendMessage(this.Clients[0], receivedMessage);
    }

    private void Clients_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        this.ClientsChanged?.Invoke(sender, e);
    }

    public void Start()
    {
        if (!this.server.IsStarted)
            this.server.Start(this.ep.Address, this.ep.Port);
    }

    public void Stop()
    {
        if (this.server.IsStarted)
            this.server.Stop();
    }

    public void SendMessage(TcpClient tcpClient, Models.Application.Message message)
    {
        byte[] data = JsonSerializer.SerializeToUtf8Bytes(message);
        if (this.server.IsStarted)
            tcpClient.GetStream().Write(data, 0, data.Length);
    }
}
