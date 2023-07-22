using Messenger.Cryptography;
using Messenger.Models.DB;
using Messenger.Repositories;
using Microsoft.IdentityModel.Tokens;
using System;

namespace Messenger.ViewModels;

public class ChangePasswordViewModel : ViewModelBase
{
    public event Action? ConfirmCompleted;
    public event Action? CompleteCancel;

    public CommandBase ConfirmCommand { get; }
    public CommandBase CancelCommand { get; }

    private string? newPassword;
    public string NewPassword
    {
        get
        { return newPassword; }
        set
        {
            newPassword = value;
            this.OnPropertyChanged();
        }
    }
    private readonly User currentUser;

    public ChangePasswordViewModel(User currentUser)
    {
        #region Initialize Commands
        this.ConfirmCommand = new(Confirm);
        this.CancelCommand = new(Cancel);
        #endregion

        this.currentUser = currentUser;
    }

    private void Confirm(object obj)
    {
        if (this.NewPassword.IsNullOrEmpty())
            return;
        var updatedUser = RepositoryFactory.GetUserRepository().GetByNickname(this.currentUser.Nickname);
        if (updatedUser is null)
            return;
        updatedUser.EncryptedPassword = HashData.EncryptData(this.NewPassword);
        RepositoryFactory.GetUserRepository().Update(updatedUser);
        this.ConfirmCompleted?.Invoke();
    }

    private void Cancel(object obj)
    {
        this.CompleteCancel?.Invoke();
    }
}
