using Messenger.Models.DB;
using Messenger.Repositories;
using Microsoft.IdentityModel.Tokens;
using System;

namespace Messenger.ViewModels;

public class ChangeNicknameViewModel : ViewModelBase
{
    public event Action? ConfirmCompleted;
    public event Action? ConfirmFailed;
    public event Action? CompleteCancel;

    public CommandBase ConfirmCommand { get; }
    public CommandBase CancelCommand { get; }

    private string? newNickname;
    public string? NewNickname
    {
        get => this.newNickname;
        set
        {
            this.newNickname = value;
            this.OnPropertyChanged();
        }
    }
    private string? oldNickname;
    public string? OldNickname
    {
        get => this.oldNickname;
        set
        {
            this.oldNickname = value;
            this.OnPropertyChanged();
        }
    }
    private readonly User currentUser;

    public ChangeNicknameViewModel(User currentUser)
    {
        #region Initialize Commands
        this.ConfirmCommand = new(this.Confirm);
        this.CancelCommand = new(this.Cancel);
        #endregion

        this.NewNickname = currentUser.Nickname;
        this.OldNickname = currentUser.Nickname;
        this.currentUser = currentUser;
    }

    private void Confirm(object obj)
    {
        if (this.NewNickname.IsNullOrEmpty()
            ||this.NewNickname!.Equals(this.OldNickname))
        {
            this.CompleteCancel?.Invoke();
            return;
        }
        var updatedUser = RepositoryFactory.GetUserRepository().GetByNickname(this.currentUser.Nickname);
        if (updatedUser is null)
        {
            this.ConfirmFailed?.Invoke();
            return;
        }
        updatedUser.Nickname = this.NewNickname;
        if (RepositoryFactory.GetUserRepository().Update(updatedUser)?.Nickname == updatedUser.Nickname)
            this.ConfirmCompleted?.Invoke();
        else
            this.ConfirmFailed?.Invoke();
    }

    private void Cancel(object obj) =>
        this.CompleteCancel?.Invoke();
}
