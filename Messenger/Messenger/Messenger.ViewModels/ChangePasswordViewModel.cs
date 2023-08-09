using Messenger.Cryptography;
using Messenger.Models.DB;
using Messenger.Repositories;
using Microsoft.IdentityModel.Tokens;
using System;

namespace Messenger.ViewModels;

public class ChangePasswordViewModel : ViewModelBase
{
    public event Action? ConfirmCompleted;
    public event Action? ConfirmFailed;
    public event Action? CompleteCancel;

    public CommandBase ConfirmCommand { get; }
    public CommandBase CancelCommand { get; }

    private string? newPassword;
    public string? NewPassword
    {
        get => this.newPassword;
        set
        {
            this.newPassword = value;
            this.OnPropertyChanged();
        }
    }
    private readonly User currentUser;

    public ChangePasswordViewModel(User currentUser)
    {
        #region Initialize Commands
        this.ConfirmCommand = new(this.Confirm);
        this.CancelCommand = new(this.Cancel);
        #endregion

        this.currentUser = currentUser;
    }

    private void Confirm(object obj)
    {
        if (this.NewPassword.IsNullOrEmpty())
        {
            this.CompleteCancel?.Invoke();
            return;
        }
        var updatedUser = RepositoryFactory.SharedUserRepository.GetByNickname(this.currentUser.Nickname);
        if (updatedUser is null)
        {
            this.ConfirmFailed?.Invoke();
            return;
        }
        updatedUser.EncryptedPassword = HashData.EncryptData(this.NewPassword!);
        if (RepositoryFactory.SharedUserRepository.Update(updatedUser)?.EncryptedPassword == updatedUser.EncryptedPassword)
            this.ConfirmCompleted?.Invoke();
        else
            this.ConfirmFailed?.Invoke();
    }

    private void Cancel(object obj) =>
        this.CompleteCancel?.Invoke();
}
