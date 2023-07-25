using Messenger.BL;
using Messenger.Models.Application;
using Messenger.Models.DB;
using Messenger.Repositories;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Messenger.ViewModels;

public class ServerViewModel : ViewModelBase
{
    public event Action? CompleteSignIn;
    public event Action? CompleteExit;
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

    private ObservableCollection<TcpClient>? clients;
    public ObservableCollection<TcpClient> Clients
    {
        get
        { return this.clients!; }
        set
        {
            this.clients = value;
            this.OnPropertyChanged();
        }
    }
    private ObservableCollection<Message>? messages;
    public ObservableCollection<Message> Messages
    {
        get
        { return this.messages!; }
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
            this.server.StateChanged += Server_StateChanged;
            this.server.MessageReceived += Server_MessageReceived;
            this.Clients = new();
            this.Messages = new();
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
    }

    private void Stop(object obj)
    {
        this.server!.Stop();
    }

    private void SignIn(object obj)
    {
        this.CompleteSignIn?.Invoke();
    }

    private void Server_ClientsChanged()
    {
        this.Clients = new(this.server!.Clients);
    }

    private void Server_StateChanged(bool isStarted)
    {
        this.IsStarted = isStarted;
    }

    private void Server_MessageReceived(Message message)
    {
        this.Messages = new(this.server!.Messages);
        this.server!.SendMessage(this.Clients[0], message);
    }
}
