using Messenger.Repositories;
using System;

namespace Messenger.ViewModels;

public class ChangeNicknameViewModel : ViewModelBase
{
    public event Action? ConfirmCompleted;
    public event Action? CompleteCancel;

    public CommandBase ConfirmCommand { get; }
    public CommandBase CancelCommand { get; }

    private string? newNickname;
    private readonly string? oldNickname;
    public string NewNickname
    {
        get
        { return newNickname; }
        set
        {
            newNickname = value;
            this.OnPropertyChanged();
        }
    }
    public string OldNickname
    {
        get
        { return this.oldNickname; }
        set
        {
            this.OnPropertyChanged();
        }
    }

    public ChangeNicknameViewModel(string oldNickname)
    {
        #region Initialize Commands
        this.ConfirmCommand = new(Confirm);
        this.CancelCommand = new(Cancel);
        #endregion

        this.NewNickname = oldNickname;
        this.oldNickname = oldNickname;
    }

    private void Confirm(object obj)
    {
        if (this.NewNickname.Equals(this.OldNickname))
            return;
        var updatedUser = RepositoryFactory.GetUserRepository().GetByNickname(this.OldNickname);
        if (updatedUser is null)
            return;
        updatedUser.Nickname = this.NewNickname;
        RepositoryFactory.GetUserRepository().Update(updatedUser);
        this.ConfirmCompleted?.Invoke();
    }

    private void Cancel(object obj)
    {
        this.CompleteCancel?.Invoke();
    }
}
