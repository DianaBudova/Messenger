using Messenger.BL;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;

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

        string? serverName = ConfigurationManager.AppSettings["ServerNameByDefault"];
        if (serverName is null)
        {
            this.CompleteExit?.Invoke();
            return;
        }
        try
        { this.server = new(serverName); }
        catch
        {
            this.CompleteExit?.Invoke();
            return;
        }
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
