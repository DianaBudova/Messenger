using Microsoft.IdentityModel.Tokens;
using System.Windows;
using Messenger.Repositories;
using Messenger.Cryptography;
using System.Net.Sockets;
using System.Net;
using System;
using Messenger.Models.DB;
using System.Configuration;
using System.Linq;

namespace Messenger.ViewModels;

public class SignUpViewModel : ViewModelBase
{
    public event Action? SignUpCompleted;
    public event Action? CompleteCancel;
    public event Action? CompleteSignIn;

    public CommandBase CreateAccountCommand { get; }
    public CommandBase CancelCommand { get; }
    public CommandBase SignInCommand { get; }

    private string? newNickname;
    private string? newPassword;
    private string? repeatedPassword;
    public string? NewNickname
    {
        get
        { return this.newNickname; }
        set
        {
            this.newNickname = value;
            this.OnPropertyChanged();
        }
    }
    public string? NewPassword
    {
        get
        { return this.newPassword; }
        set
        {
            this.newPassword = value;
            this.OnPropertyChanged();
        }
    }
    public string? RepeatedPassword
    {
        get
        { return this.repeatedPassword; }
        set
        {
            this.repeatedPassword = value;
            this.OnPropertyChanged();
        }
    }

    public SignUpViewModel()
    {
        this.CreateAccountCommand = new(this.CreateAccount);
        this.CancelCommand = new(this.Cancel);
        this.SignInCommand = new(this.SignIn);
    }

    private void CreateAccount(object obj)
    {
        if (this.NewNickname.IsNullOrEmpty() ||
            this.NewPassword.IsNullOrEmpty() ||
            this.RepeatedPassword.IsNullOrEmpty())
        {
            MessageBox.Show("Login or password are empty.", "", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        if (!this.NewPassword!.Equals(this.RepeatedPassword))
        {
            MessageBox.Show("New password and repeated password do not match.", "", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        string ipAddress = Dns.GetHostAddresses(Dns.GetHostName())
            .Where(ipAddress => ipAddress.AddressFamily == AddressFamily.InterNetwork)
            .First()
            .ToString();
        User newUser = new()
        {
            Nickname = this.NewNickname!,
            EncryptedPassword = HashData.EncryptData(this.NewPassword!),
            ProfilePhoto = System.IO.File.ReadAllBytes(ConfigurationManager.AppSettings["ImagesPath"] + "UnknownUser.png"),
            IpAddress = ipAddress,
        };
        if (RepositoryFactory.GetUserRepository().Add(newUser) is null)
        {
            MessageBox.Show("Some error occurred while adding a new user.", "", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        Server? serverByDefault = RepositoryFactory.GetServerRepository().GetByNameServer(ConfigurationManager.AppSettings["ServerNameByDefault"]);
        if (serverByDefault is null)
        {
            MessageBox.Show("Server was not set as default.", "", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        newUser.LastUsingServer = serverByDefault;
        if (RepositoryFactory.GetUserRepository().Update(newUser) is null)
        {
            MessageBox.Show("Server did not set as default.", "", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        MessageBox.Show("New user was registered successfully.", "", MessageBoxButton.OK, MessageBoxImage.Information);
        this.SignUpCompleted?.Invoke();
    }

    private void Cancel(object obj)
    {
        if (MessageBox.Show("Do you want to exit?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            this.CompleteCancel?.Invoke();
    }

    private void SignIn(object obj)
    {
        this.CompleteSignIn?.Invoke();
    }
}
