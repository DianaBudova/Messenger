using Messenger.BL;
using Messenger.Models.Application;
using Messenger.Models.DB;
using System;
using System.Windows;
using System.Collections.ObjectModel;
using Messenger.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Linq;
using Messenger.Common;
using System.Threading;

namespace Messenger.ViewModels;

public class MainViewModel : ViewModelBase
{
    public event Action? CompleteChangeNickname;
    public event Action? CompleteChangePassword;
    public event Action? CompleteVoiceRecord;
    public event Action? CompleteAttachFile;
    public event Action? CompleteExit;
    public event Action<Message>? MessageReceived;
    public event Func<byte[]?>? CompleteChangeProfilePhoto;

    public CommandBase SearchUserCommand { get; }
    public CommandBase SendMessageCommand { get; }
    public CommandBase VoiceRecordCommand { get; }
    public CommandBase AttachFileCommand { get; }
    public CommandBase ChangeNicknameCommand { get; }
    public CommandBase ChangePasswordCommand { get; }
    public CommandBase ChangeProfilePhotoCommand { get; }
    public CommandBase ClearProfilePhotoCommand { get; }
    public CommandBase DeleteAccountCommand { get; }

    private string? searchedUser;
    public string? SearchedUser
    {
        get
        { return this.searchedUser; }
        set
        {
            this.searchedUser = value;
            this.OnPropertyChanged();
        }
    }
    private string? inputMessage;
    public string? InputMessage
    {
        get
        { return this.inputMessage; }
        set
        {
            this.inputMessage = value;
            this.OnPropertyChanged();
        }
    }
    private string? nickname;
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
    private byte[]? profilePhoto;
    public byte[]? ProfilePhoto
    {
        get
        { return this.profilePhoto; }
        set
        {
            this.profilePhoto = value;
            OnPropertyChanged();
        }
    }
    private User? selectedUser;
    public User? SelectedUser
    {
        get
        { return this.selectedUser; }
        set
        {
            this.selectedUser = value;
            OnPropertyChanged();
            Application.Current.Dispatcher.Invoke(this.Messages.Clear);
            if (this.SelectedUser is not null)
            {
                var messages = RepositoryFactory.GetChatRepository().GetBySenderRecipientId(this.SignedUser.Id, this.SelectedUser.Id);
                if (messages is null)
                    return;
                foreach (var message in messages)
                    Application.Current.Dispatcher.Invoke(() => this.Messages.Add(message));
            }
        }
    }
    private Chat? selectedMessage;
    public Chat? SelectedMessage
    {
        get
        { return this.selectedMessage; }
        set
        {
            this.selectedMessage = value;
            OnPropertyChanged();
        }
    }
    private MultimediaMessage? messageToSend;
    public MultimediaMessage? MultimediaMessage
    {
        get
        { return this.messageToSend; }
        set
        {
            this.messageToSend = value;
            this.OnPropertyChanged();
        }
    }

    private ObservableCollection<User>? users;
    public ObservableCollection<User> Users
    {
        get
        { return this.users; }
        set
        {
            this.users = value;
            OnPropertyChanged();
        }
    }
    private ObservableCollection<User>? tempUsers;
    private ObservableCollection<Chat>? messages;
    public ObservableCollection<Chat> Messages
    {
        get
        { return this.messages; }
        set
        {
            this.messages = value;
            OnPropertyChanged();
        }
    }

    public readonly User SignedUser;
    private readonly TCPClient client;
    private readonly VoiceRecorderAdapter recorder;

    public MainViewModel(User signedUser)
    {
        RepositoryFactory.GetUserRepository().Update(signedUser);

        #region Initialize Commands
        this.SearchUserCommand = new(this.SearchUser);
        this.SendMessageCommand = new(this.SendMessage);
        this.VoiceRecordCommand = new(this.VoiceRecord);
        this.AttachFileCommand = new(this.AttachFile);
        this.ChangeNicknameCommand = new(this.ChangeNickname);
        this.ChangePasswordCommand = new(this.ChangePassword);
        this.ChangeProfilePhotoCommand = new(this.ChangeProfilePhoto);
        this.ClearProfilePhotoCommand = new(this.ClearProfilePhoto);
        this.DeleteAccountCommand = new(this.DeleteAccount);
        #endregion

        this.SignedUser = signedUser;
        this.Nickname = signedUser.Nickname;
        this.ProfilePhoto = signedUser.ProfilePhoto;
        this.Users = new();
        this.Messages = new();
        this.recorder = new();
        this.StartMonitoringUsersAsync();
        try
        {
            string? serverName = this.SignedUser.LastUsingServer?.NameServer ?? ConfigurationManager.AppSettings["ServerNameByDefault"];
            if (serverName is null)
                throw new Exception();
            Server? chosenServer = RepositoryFactory.GetServerRepository().GetByNameServer(serverName)
                ?? throw new Exception();
            IPEndPoint ep = new(IPAddress.Parse(chosenServer.IpAddress), chosenServer.Port);
            this.client = new(ep);
        }
        catch
        {
            Environment.Exit(0);
        }
        this.client.MessageReceived += Client_MessageReceived;
    }

    private async void StartMonitoringUsersAsync()
    {
        await Task.Run(() =>
        {
            while (true)
            {
                List<User> users = RepositoryFactory.GetUserRepository().GetAll();
                if (users is null || users.Count == 0)
                    continue;
                users.Remove(users.Find(u => u.IsSimilar(this.SignedUser)));
                users.Remove(users.Find(u => u.Port is null));
                User? selUser = null;
                if (this.SelectedUser is not null)
                {
                    selUser = new()
                    {
                        Id = this.SelectedUser.Id,
                        Nickname = this.SelectedUser.Nickname,
                        EncryptedPassword = this.SelectedUser.EncryptedPassword,
                        IpAddress = this.SelectedUser.IpAddress,
                        Port = this.SelectedUser.Port,
                        ProfilePhoto = this.SelectedUser.ProfilePhoto,
                        LastUsingServer = this.SelectedUser.LastUsingServer,
                    };
                }
                Application.Current.Dispatcher.Invoke(this.Users.Clear);
                foreach (var user in users)
                    Application.Current.Dispatcher.Invoke(() => this.Users.Add(user));
                this.SelectedUser = this.Users.Where(user => user.IsSimilar(selUser)).FirstOrDefault();
                Thread.Sleep(1000);
            }
        });
    }

    public void ConnectToServer()
    {
        try
        {
            TcpClient? client = this.client.Connect();
            if (client is null)
                throw new ArgumentNullException(nameof(client));
            int port = ((IPEndPoint)client.Client.LocalEndPoint!).Port;
            if (port <= 0)
                throw new ArgumentNullException(nameof(port));
            User? existedUser = RepositoryFactory.GetUserRepository().GetByNickname(this.SignedUser.Nickname);
            if (existedUser is null)
                throw new ArgumentNullException(nameof(existedUser));
            existedUser.Port = port;
            RepositoryFactory.GetUserRepository().Update(existedUser);
        }
        catch
        {
            if (this.CompleteExit is not null)
                this.CompleteExit.Invoke();
            else
                Environment.Exit(0);
        }
    }

    public void DisconnectFromServer()
    {
        try
        {
            this.client.Disconnect();
            User? existedUser = RepositoryFactory.GetUserRepository().GetByNickname(this.SignedUser.Nickname);
            if (existedUser is null)
                return;
            existedUser.Port = null;
            RepositoryFactory.GetUserRepository().Update(existedUser);
        }
        catch
        { }
    }

    private void SearchUser(object obj)
    {
        try
        {
            if (this.SearchedUser.IsNullOrEmpty())
            {
                if (this.tempUsers is not null || this.tempUsers?.Count > 0)
                {
                    this.Users = new(this.tempUsers);
                    this.tempUsers.Clear();
                }
                return;
            }
            var allUsers = RepositoryFactory.GetUserRepository().GetAll();
            if (allUsers is null)
                return;
            allUsers.Remove(allUsers.Find(user => this.SignedUser.IsSimilar(user)));
            if (this.tempUsers is null || this.tempUsers.Count == 0)
                this.tempUsers = new(this.Users);
            List<User> searchedUsers = new();
            foreach (var user in allUsers)
                if (user.Nickname.Contains(this.SearchedUser!))
                    searchedUsers.Add(user);
            this.Users.Clear();
            this.Users = new(searchedUsers);
        }
        catch
        {
            MessageBox.Show("Some error occured.", "",
                MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
    }

    private void SendMessage(object obj)
    {
        if (this.SelectedUser is null)
            return;
        Message messageToSend = new()
        {
            Sender = this.SignedUser,
            Recipient = this.SelectedUser,
            DateTime = DateTime.Now,
        };
        try
        {
            if (this.MultimediaMessage is not null && !this.MultimediaMessage.Value.Content.IsNullOrEmpty())
            {
                messageToSend.Content = this.MultimediaMessage.Value.Content;
                messageToSend.Type = this.MultimediaMessage.Value.Type == MultimediaMessageType.File ? MessageType.File : MessageType.Audio;
            }
            else if (!this.InputMessage.IsNullOrEmpty())
            {
                messageToSend.Content = JsonSerializer.SerializeToUtf8Bytes(this.InputMessage);
                messageToSend.Type = MessageType.Text;
            }
            else
                throw new Exception();
            this.client.SendMessage(messageToSend);
        }
        catch
        {
            MessageBox.Show("Server is not working at the moment or there is no message to send.", "Error occurred when sending a message",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void VoiceRecord(object obj)
    {
        if (!this.recorder.CanStartRecording())
            return;
        if (MessageBox.Show("Click OK to start recording voice message.", "Start recording voice message?",
            MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
            this.CompleteVoiceRecord?.Invoke();
    }

    private void AttachFile(object obj)
    {
        this.CompleteAttachFile?.Invoke();
    }

    private void ChangeNickname(object obj)
    {
        int currentUserId = RepositoryFactory.GetUserRepository().GetByNickname(this.SignedUser.Nickname).Id;
        this.CompleteChangeNickname?.Invoke();
        this.SignedUser.Nickname = RepositoryFactory.GetUserRepository().GetById(currentUserId)!.Nickname;
        this.Nickname = this.SignedUser.Nickname;
    }

    private void ChangePassword(object obj)
    {
        int currentUserId = RepositoryFactory.GetUserRepository().GetByNickname(this.SignedUser.Nickname).Id;
        this.CompleteChangePassword?.Invoke();
        this.SignedUser.EncryptedPassword = RepositoryFactory.GetUserRepository().GetById(currentUserId)!.EncryptedPassword;
    }

    private void ChangeProfilePhoto(object obj)
    {
        byte[]? image = this.CompleteChangeProfilePhoto?.Invoke();
        if (image is null)
            return;
        this.SignedUser.ProfilePhoto = image;
        this.ProfilePhoto = image;
        var updatedUser = RepositoryFactory.GetUserRepository().GetByNickname(this.SignedUser.Nickname);
        if (updatedUser is null)
            return;
        updatedUser.ProfilePhoto = image;
        RepositoryFactory.GetUserRepository().Update(updatedUser);
    }

    private void ClearProfilePhoto(object obj)
    {
        if (MessageBox.Show("Are you sure you want to clear the profile photo?", "To clear the profile photo?",
            MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            return;
        this.SignedUser.ProfilePhoto = System.IO.File.ReadAllBytes(ConfigurationManager.AppSettings["ImagesPath"] + "UnknownUser.png");
        this.ProfilePhoto = System.IO.File.ReadAllBytes(ConfigurationManager.AppSettings["ImagesPath"] + "UnknownUser.png");
        var updatedUser = RepositoryFactory.GetUserRepository().GetByNickname(this.SignedUser.Nickname);
        if (updatedUser is null)
        {
            MessageBox.Show("Such a user does not exist in the database.", "",
                MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        updatedUser.ProfilePhoto = System.IO.File.ReadAllBytes(ConfigurationManager.AppSettings["ImagesPath"] + "UnknownUser.png");
        RepositoryFactory.GetUserRepository().Update(updatedUser);
    }

    public void DeleteAccount(object obj)
    {
        if (MessageBox.Show("Are you sure you want to delete the current account?", "To delete the account?",
            MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            return;
        var deletedUser = RepositoryFactory.GetUserRepository().GetByNickname(this.SignedUser.Nickname);
        if (deletedUser is null)
        {
            MessageBox.Show("Such a user does not exist in the database.", "",
                MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        RepositoryFactory.GetUserRepository().Remove(deletedUser);
        MessageBox.Show("User was deleted successfully!", "",
            MessageBoxButton.OK, MessageBoxImage.Information);
        RepositoryFactory.GetUserRepository().Update(new(RepositoryFactory.GetUserRepository().GetByNickname(this.SignedUser.Nickname)) { Port = null });
        this.client.Disconnect();
        this.CompleteExit?.Invoke();
    }

    private void Client_MessageReceived(Message receivedMessage)
    {
        if (receivedMessage.Type != MessageType.EndOfLine)
            this.MessageReceived?.Invoke(receivedMessage);
        else
        {
            MessageBox.Show("Server is shut down which you were connected. Try again later.", "Server shut down",
                MessageBoxButton.OK, MessageBoxImage.Information);
            if (this.CompleteExit is not null)
                this.CompleteExit.Invoke();
            else
                Environment.Exit(0);
        }
    }
}
