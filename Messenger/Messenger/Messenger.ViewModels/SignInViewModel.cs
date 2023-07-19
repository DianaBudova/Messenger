using System.Windows;
using Messenger.Repositories;
using Messenger.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System;
using Messenger.Models.DB;

namespace Messenger.ViewModels;

public class SignInViewModel : ViewModelBase
{
    public event Action? SignInCompleted;
    public event Action? CompleteSignUp;
    public event Action? CompleteCancel;

    public CommandBase LoginCommand { get; }
    public CommandBase CancelCommand { get; }
    public CommandBase CreateAccountCommand { get; }

    private string? nickname;
    private string? password;
    public string? Nickname
    {
        get
        { return this.nickname; }
        set
        {
            this.nickname = value;
            this.OnPropertyChanged();
        }
    }
    public string? Password
    {
        get
        { return this.password; }
        set
        {
            this.password = value;
            this.OnPropertyChanged();
        }
    }

    public SignInViewModel()
    {
        this.LoginCommand = new(this.Login);
        this.CancelCommand = new(this.Cancel);
        this.CreateAccountCommand = new(this.CreateAccount);
    }

    private void Login(object obj)
    {
        if (this.Nickname.IsNullOrEmpty() ||
            this.Password.IsNullOrEmpty())
        {
            MessageBox.Show("Login or password are empty.", "", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        User? user = RepositoryFactory.GetUserRepository().GetByNickname(this.Nickname!);
        if (user is null)
        {
            MessageBox.Show("Nickname was entered incorrectly.", "", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        if (!HashData.VerifyData(user.EncryptedPassword, this.Password!))
        {
            MessageBox.Show("Password was entered incorrectly.", "", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        MessageBox.Show("Login successfully.", "", MessageBoxButton.OK, MessageBoxImage.Information);
        this.SignInCompleted?.Invoke();
    }

    private void Cancel(object obj)
    {
        if (MessageBox.Show("Do you want to exit?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            this.CompleteCancel?.Invoke();
    }

    private void CreateAccount(object obj)
    {
        this.CompleteSignUp?.Invoke();
    }
}
