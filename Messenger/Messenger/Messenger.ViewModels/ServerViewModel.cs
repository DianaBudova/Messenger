using Messenger.BL;
using Messenger.Models.DB;
using Messenger.Repositories;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Windows.Forms;

namespace Messenger.ViewModels;

public class ServerViewModel : ViewModelBase
{
    public event Action? CompleteSignIn;
    public event Action<string>? ServerChanged;
    public CommandBase StartCommand { get; }
    public CommandBase StopCommand { get; }
    public CommandBase SignInCommand { get; }

    private string? ipAddress;
    private int port;
    private string? serverName;
    private string? selectedClient;
    private bool isStarted;
    public string IpAddress
    {
        get
        { return this.ipAddress; }
        set
        {
            this.ipAddress = value;
            this.OnPropertyChanged();
        }
    }
    public int Port
    {
        get
        { return this.port; }
        set
        {
            this.port = value;
            this.OnPropertyChanged();
        }
    }
    public string ServerName
    {
        get
        { return this.serverName; }
        set
        {
            this.serverName = value;
            this.OnPropertyChanged();
        }
    }
    public string SelectedClient
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
            if (this.receivedMessage is not null)
                this.server.SendMessage(this.clients[0], this.ReceivedMessage!.Value);
            this.OnPropertyChanged();
        }
    }

    private ObservableCollection<string>? clients;
    public ObservableCollection<string> Clients
    {
        get
        { return this.clients; }
        set
        {
            this.clients = value;
            this.OnPropertyChanged();
        }
    }

    private readonly TCPServer server;

    public ServerViewModel()
    {
        #region Initialize Commands
        this.StartCommand = new(this.Start);
        this.StopCommand = new(this.Stop);
        this.SignInCommand = new(this.SignIn);
        #endregion

        Server? server = RepositoryFactory.GetServerRepository().GetByNameServer(ConfigurationManager.AppSettings["ServerNameByDefault"]);
        if (server is null)
        {
            MessageBox.Show("Messenger was not succeded in connecting to server", "Server was not found",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            Environment.Exit(0);
        }
        this.IpAddress = server.IpAddress;
        this.Port = server.Port;
        this.ServerName = server.NameServer;
        this.server = new(this.ipAddress!, this.port);
        this.server.ClientsChanged += Server_ClientsChanged;
        this.server.MessageReceived += Server_MessageReceived;
    }

    private void Server_ClientsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        this.Clients = new(this.server.Clients);
    }

    private void Server_MessageReceived(Models.Application.Message message)
    {
        this.ReceivedMessage = new(message);
    }

    private void Start(object obj)
    {
        this.ServerChanged?.Invoke(this.ServerName);
        this.server.Start();
        this.IsStarted = true;
    }

    private void Stop(object obj)
    {
        this.server.Stop();
        this.IsStarted = false;
    }

    private void SignIn(object obj)
    {
        this.CompleteSignIn?.Invoke();
    }
}
