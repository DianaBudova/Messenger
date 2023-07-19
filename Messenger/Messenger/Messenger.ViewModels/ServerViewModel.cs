using Messenger.BL;
using Messenger.Models.Application;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Messenger.ViewModels;

public class ServerViewModel : ViewModelBase
{
    public event Action? CompleteSignIn;
    public CommandBase StartCommand { get; }
    public CommandBase StopCommand { get; }
    public CommandBase SendCommand { get; }
    public CommandBase SignInCommand { get; }

    private string? ipAddress;
    private int port;
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
        this.SendCommand = new(this.Send);
        this.SignInCommand = new(this.SignIn);
        #endregion

        this.IpAddress = "127.0.0.1";
        this.Port = 8888;
        this.server = new(this.ipAddress!, this.port);
        this.server.ClientConnected += Server_ClientConnected;
        this.server.ClientDisconnected += Server_ClientDisconnected;
        this.server.MessageReceived += Server_MessageReceived;
        this.Clients = new();
    }

    private void Server_ClientConnected()
    {
        this.Clients = new(this.server.Clients);
    }

    private void Server_ClientDisconnected(string ipPort)
    {
        this.Clients.Remove(ipPort);
    }

    private void Server_MessageReceived(Message message)
    {
        
    }

    private void Start(object obj)
    {
        this.server.Start();
    }

    private void Stop(object obj)
    {
        this.server.Stop();
    }

    private void Send(object obj)
    {
        //this.server.SendMessage(new Message());
    }

    private void SignIn(object obj)
    {
        this.CompleteSignIn?.Invoke();
    }
}
