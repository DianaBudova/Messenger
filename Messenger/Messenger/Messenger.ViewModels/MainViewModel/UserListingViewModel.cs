using Messenger.Models.DB;
using Messenger.Repositories;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using User = Messenger.Models.DB.User;

namespace Messenger.ViewModels.MainViewModel;

public class UserListingViewModel : ViewModelBase
{
    private User? selectedUser;
    public User? SelectedUser
    {
        get => selectedUser;
        set
        {
            this.selectedUser = value;
            this.OnPropertyChanged();
        }
    }
    private Chat? selectedMessage;
    public Chat? SelectedMessage
    {
        get => this.selectedMessage;
        set
        {
            this.selectedMessage = value;
            this.OnPropertyChanged();
        }
    }
    private ObservableCollection<User>? users;
    public ObservableCollection<User> Users
    {
        get => this.users;
        set
        {
            this.users = value;
            this.OnPropertyChanged();
        }
    }
    public bool HasUsers => this.users?.Count > 0;
    private readonly User signedUser;

    public UserListingViewModel(User signedUser)
    {
        this.Users = new();
        this.signedUser = signedUser;
        Task.Run(this.StartMonitoringUsers);
    }

    private async Task StartMonitoringUsers()
    {
        while (true)
        {
            List<User>? users = RepositoryFactory.GetUserRepository().GetAll((user) => user is not null && user.Port.HasValue && !user.Equals(this.signedUser));
            if (users is null || users.Count == 0)
                continue;
            User? tempSelectedUser = this.SelectedUser is null ? null : this.SelectedUser;
            //Chat? tempSelectedMessage = this.SelectedMessage is null ? null : this.SelectedMessage;
            Application.Current.Dispatcher.Invoke(this.Users!.Clear);
            foreach (var user in users)
                Application.Current.Dispatcher.Invoke(() => this.Users!.Add(user));
            this.SelectedUser = tempSelectedUser;
            await Task.Delay(1000);
        }
    }
}
