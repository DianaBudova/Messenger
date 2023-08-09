using System.Windows;
using Messenger.Repositories;
using Messenger.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System;
using Messenger.Models.DB;
using System.Net;
using System.Linq;
using System.Net.Sockets;

namespace Messenger.ViewModels;

public class SignInViewModel : ViewModelBase
{
    public event Action<User>? SignInCompleted;
    public event Action? CompleteSignUp;
    public event Action? CompleteCancel;

    public CommandBase LoginCommand { get; }
    public CommandBase CancelCommand { get; }
    public CommandBase CreateAccountCommand { get; }

    private string? nickname;
    public string? Nickname
    {
        get => this.nickname;
        set
        {
            this.nickname = value;
            this.OnPropertyChanged();
        }
    }
    private string? password;
    public string? Password
    {
        get => this.password;
        set
        {
            this.password = value;
            this.OnPropertyChanged();
        }
    }
    private string? lastUsingServer;
    public string? LastUsingServer
    {
        get => this.lastUsingServer;
        set
        {
            this.lastUsingServer = value;
            this.OnPropertyChanged();
        }
    }

    public SignInViewModel()
    {
        #region Initialize Commands
        this.LoginCommand = new(this.Login);
        this.CancelCommand = new(this.Cancel);
        this.CreateAccountCommand = new(this.CreateAccount);
        #endregion
    }

    private void Login(object obj)
    {
        if (this.LastUsingServer.IsNullOrEmpty())
        {
            MessageBox.Show("Server is not chosen. Please choose a server.", "", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        if (this.Nickname.IsNullOrEmpty())
        {
            MessageBox.Show("Login is empty.", "", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        if (this.Password.IsNullOrEmpty())
        {
            MessageBox.Show("Password is empty.", "", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        User? user = RepositoryFactory.GetUserRepository().GetByNickname(this.Nickname!);
        if (user is null)
        {
            MessageBox.Show("Nickname was entered incorrectly.", "", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        if (user.Port.HasValue)
        {
            MessageBox.Show("This user is already online.", "", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        if (!HashData.VerifyData(user.EncryptedPassword, this.Password!))
        {
            MessageBox.Show("Password was entered incorrectly.", "", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        MessageBox.Show("Login successfully.", "", MessageBoxButton.OK, MessageBoxImage.Information);
        user.LastUsingServer = RepositoryFactory.GetServerRepository().GetByNameServer(this.LastUsingServer!);
        user.IpAddress = Dns.GetHostAddresses(Dns.GetHostName())
            .Where(ipAddress => ipAddress.AddressFamily == AddressFamily.InterNetwork)
            .First()
            .ToString();
        this.SignInCompleted?.Invoke(user);
    }

    private void Cancel(object obj)
    {
        if (MessageBox.Show("Do you want to exit?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            this.CompleteCancel?.Invoke();
    }

    private void CreateAccount(object obj) =>
        this.CompleteSignUp?.Invoke();
}
