using Messenger.BL;
using Messenger.Models.Application;
using Messenger.Models.DB;
using Messenger.Repositories;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Net;
using System.Net.Sockets;

namespace Messenger.ViewModels;

public class ServerViewModel : ViewModelBase
{
    public event Action? CompleteExit;

    public CommandBase StartCommand { get; }
    public CommandBase StopCommand { get; }

    private bool isStarted;
    public bool IsStarted
    {
        get => this.isStarted;
        set
        {
            this.isStarted = value;
            this.OnPropertyChanged();
        }
    }
    private ObservableCollection<TcpClient>? clients;
    public ObservableCollection<TcpClient>? Clients
    {
        get => this.clients;
        set
        {
            this.clients = value;
            this.OnPropertyChanged();
        }
    }
    private ObservableCollection<Message>? messages;
    public ObservableCollection<Message>? Messages
    {
        get => this.messages;
        set
        {
            this.messages = value;
            this.OnPropertyChanged();
        }
    }
    private readonly TCPServer? server;

    public ServerViewModel()
    {
        #region Initialize Commands
        this.StartCommand = new(this.Start);
        this.StopCommand = new(this.Stop);
        #endregion

        try
        {
            string? serverName = ConfigurationManager.AppSettings["ServerNameByDefault"] 
                ?? throw new ArgumentNullException(nameof(serverName));
            Server? chosenServer = RepositoryFactory.GetServerRepository().GetByNameServer(serverName)
                ?? throw new ArgumentNullException(nameof(chosenServer));
            IPEndPoint ep = new(IPAddress.Parse(chosenServer!.IpAddress), chosenServer.Port);
            this.server = new(ep);
            this.Clients = new();
            this.Messages = new();
        }
        catch
        { Environment.Exit(0); }
        this.server.ClientsChanged += this.Server_ClientsChanged;
        this.server.StateChanged += this.Server_StateChanged;
        this.server.MessageReceived += this.Server_MessageReceived;
    }

    private void Start(object obj) =>
        this.server!.Start();

    private void Stop(object obj) =>
        this.server!.Stop();

    private void Server_ClientsChanged() =>
        this.Clients = new(this.server!.Clients);

    private void Server_StateChanged(bool isStarted) =>
        this.IsStarted = isStarted;

    private void Server_MessageReceived(Message message)
    {
        this.Messages = new(this.server!.Messages);
        if (message.Recipient is null || message.Recipient.Port is null)
            return;
        try
        {
            using TcpClient recipient = new();
            recipient.Connect(IPAddress.Parse(message.Recipient.IpAddress), message.Recipient.Port.Value);
            this.server!.SendMessage(recipient, message);
        }
        catch { }
    }
}
