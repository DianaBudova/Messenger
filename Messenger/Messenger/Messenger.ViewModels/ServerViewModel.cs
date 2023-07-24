using Messenger.BL;
using Messenger.Models.DB;
using Messenger.Repositories;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Net;
using System.Net.Sockets;

namespace Messenger.ViewModels;

public class ServerViewModel : ViewModelBase
{
    public event Action? CompleteSignIn;
    public event Action? CompleteExit;
    public event Action<string>? ServerChanged;
    public CommandBase StartCommand { get; }
    public CommandBase StopCommand { get; }
    public CommandBase SignInCommand { get; }

    private string? selectedClient;
    private bool isStarted;
    public string? SelectedClient
    {
        get
        { return this.selectedClient; }
        set
        {
            this.selectedClient = value;
            this.OnPropertyChanged();
        }
    }
    public bool IsStarted
    {
        get
        { return this.isStarted; }
        set
        {
            this.isStarted = value;
            this.OnPropertyChanged();
        }
    }
    private Models.Application.Message? receivedMessage;
    public Models.Application.Message? ReceivedMessage
    {
        get
        { return this.receivedMessage; }
        set
        {
            this.receivedMessage = value;
            //if (this.receivedMessage is not null)
            //    this.server!.SendMessage(this.clients[0], this.ReceivedMessage!.Value);
            this.OnPropertyChanged();
        }
    }

    private ObservableCollection<TcpClient>? clients;
    public ObservableCollection<TcpClient>? Clients
    {
        get
        { return this.clients; }
        set
        {
            this.clients = value;
            this.OnPropertyChanged();
        }
    }

    private readonly TCPServer? server;

    public ServerViewModel()
    {
        #region Initialize Commands
        this.StartCommand = new(this.Start);
        this.StopCommand = new(this.Stop);
        this.SignInCommand = new(this.SignIn);
        #endregion

        this.isStarted = false;
        try
        {
            string? serverName = ConfigurationManager.AppSettings["ServerNameByDefault"] 
                ?? throw new ArgumentNullException(nameof(serverName));
            Server? chosenServer = RepositoryFactory.GetServerRepository().GetByNameServer(serverName)
                ?? throw new ArgumentNullException(nameof(chosenServer));
            IPEndPoint ep = new(IPAddress.Parse(chosenServer!.IpAddress), chosenServer.Port);
            this.server = new(ep);
            this.server.ClientsChanged += Server_ClientsChanged;
            this.server.MessageReceived += Server_MessageReceived;
        }
        catch
        {
            this.CompleteExit?.Invoke();
            return;
        }
    }

    private void Start(object obj)
    {
        this.server!.Start();
        this.IsStarted = true;
    }

    private void Stop(object obj)
    {
        this.server!.Stop();
        this.IsStarted = false;
    }

    private void SignIn(object obj)
    {
        this.CompleteSignIn?.Invoke();
    }

    private void Server_ClientsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        this.Clients = new(this.server!.Clients);
    }

    private void Server_MessageReceived(Models.Application.Message message)
    {
        this.ReceivedMessage = new(message);
    }
}
