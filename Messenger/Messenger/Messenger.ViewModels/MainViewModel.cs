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
using System.Linq;
using Messenger.Common;

namespace Messenger.ViewModels;

public class MainViewModel : ViewModelBase
{
    public event Action? CompleteChangeNickname;
    public event Action? CompleteChangePassword;
    public event Action? CompleteVoiceRecord;
    public event Action? CompleteAttachFile;
    public event Action? MessageSendCompleted;
    public event Action? CompleteFailed;
    public event Action? CompleteExit;
    public event Func<byte[]?>? CompleteChangeProfilePhoto;

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
        get => this.searchedUser;
        set
        {
            this.searchedUser = value;
            this.OnPropertyChanged();
        }
    }
    private string? inputMessage;
    public string? InputMessage
    {
        get => this.inputMessage;
        set
        {
            this.inputMessage = value;
            this.OnPropertyChanged();
        }
    }
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
    private byte[]? profilePhoto;
    public byte[]? ProfilePhoto
    {
        get => this.profilePhoto;
        set
        {
            this.profilePhoto = value;
            this.OnPropertyChanged();
        }
    }
    private MultimediaMessage? multimediaMessage;
    public MultimediaMessage? MultimediaMessage
    {
        get => this.multimediaMessage;
        set
        {
            this.multimediaMessage = value;
            this.OnPropertyChanged();
        }
    }
    private User? selectedUser;
    public User? SelectedUser
    {
        get => this.selectedUser;
        set
        {
            this.selectedUser = value;
            this.OnPropertyChanged();
            if (this.SelectedUser is not null)
            {
                try
                {
                    var sentMessages = RepositoryFactory.GetChatRepository().GetBySenderRecipientId(this.SignedUser.Id, this.SelectedUser!.Id);
                    var receivedMessages = RepositoryFactory.GetChatRepository().GetBySenderRecipientId(this.SelectedUser.Id, this.SignedUser.Id);
                    if (sentMessages is null || receivedMessages is null)
                        return;
                    var allMessages = sentMessages.Concat(receivedMessages).OrderBy(item => item.DateTime).ToList();
                    Application.Current.Dispatcher.Invoke(() => this.Messages = new(allMessages));
                }
                catch { }
            }
            else
                Application.Current.Dispatcher.Invoke(() => this.Messages?.Clear());
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
    public ObservableCollection<User>? Users
    {
        get => this.users;
        set
        {
            this.users = value;
            this.OnPropertyChanged();
        }
    }
    private ObservableCollection<Chat>? messages;
    public ObservableCollection<Chat>? Messages
    {
        get => this.messages;
        set
        {
            this.messages = value;
            this.OnPropertyChanged();
        }
    }
    public readonly User SignedUser;
    private readonly TCPClient client;
    private readonly VoiceRecorderAdapter recorder;
    private readonly int millisecondsDelay = 1000;

    public MainViewModel(User signedUser)
    {
        RepositoryFactory.GetUserRepository().Update(signedUser);

        #region Initialize Commands
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
        this.Messages = new();
        this.recorder = new();
        try
        {
            string? serverName = this.SignedUser.LastUsingServer?.NameServer ?? ConfigurationManager.AppSettings["ServerNameByDefault"]
                ?? throw new Exception();
            Server? chosenServer = RepositoryFactory.GetServerRepository().GetByNameServer(serverName)
                ?? throw new Exception();
            IPEndPoint ep = new(IPAddress.Parse(chosenServer.IpAddress), chosenServer.Port);
            this.client = new(ep);
        }
        catch
        { Environment.Exit(0); }
        this.client.MessageReceived += this.Client_MessageReceived;
        Task.Run(this.StartMonitoringUsers);
    }

    private async Task StartMonitoringUsers()
    {
        while (true)
        {
            List<User>? users = RepositoryFactory.GetUserRepository().GetAll(this.IsUserMatch);
            if (users is not null && users.Count > 0)
            {
                User? tempSelectedUser = SelectedUser is null ? null : SelectedUser;
                Chat? tempSelectedMessage = SelectedMessage is null ? null : SelectedMessage;
                Application.Current.Dispatcher.Invoke(() => this.Users = new(users));
                this.SelectedUser = this.Users!.Where(user => user.Equals(tempSelectedUser)).FirstOrDefault();
                this.SelectedMessage = this.Messages!.Where(msg => msg.Equals(tempSelectedMessage)).FirstOrDefault();
            }
            else
                Application.Current.Dispatcher.Invoke(() => this.Users?.Clear());
            await Task.Delay(this.millisecondsDelay);
        }
    }

    private bool IsUserMatch(User? user) =>
        user is not null && !user.Equals(this.SignedUser) && (this.SearchedUser.IsNullOrEmpty() || user.Nickname.Contains(this.SearchedUser!));

    public void ConnectToServer()
    {
        try
        {
            TcpClient? client = this.client.Connect();
            if (client is null)
                throw new ArgumentNullException(nameof(client));
            int port = ((IPEndPoint)client.Client.LocalEndPoint!).Port;
            if (!PortHelper.IsPortAppropriate(port))
                throw new ArgumentNullException(nameof(port));
            User? existedUser = RepositoryFactory.GetUserRepository().GetByNickname(this.SignedUser.Nickname);
            if (existedUser is null)
                throw new ArgumentNullException(nameof(existedUser));
            this.SignedUser.Port = existedUser.Port = port;
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

    private void SendMessage(object obj)
    {
        if (this.SelectedUser is null)
            return;
        Message messageToSend = new()
        {
            Sender = SignedUser,
            Recipient = SelectedUser,
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
            this.MessageSendCompleted?.Invoke();
        }
        catch
        {
            MessageBox.Show("Server is not working at the moment or there is no message to send.", "Error occurred when sending a message",
                MessageBoxButton.OK, MessageBoxImage.Error);
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

    private void AttachFile(object obj) =>
        this.CompleteAttachFile?.Invoke();

    private void ChangeNickname(object obj)
    {
        int? currentUserId = RepositoryFactory.GetUserRepository().GetByNickname(SignedUser.Nickname)?.Id;
        if (currentUserId.HasValue)
        {
            this.CompleteFailed?.Invoke();
            return;
        }
        this.CompleteChangeNickname?.Invoke();
        this.SignedUser.Nickname = RepositoryFactory.GetUserRepository().GetById(currentUserId!.Value)!.Nickname;
        this.Nickname = this.SignedUser.Nickname;
    }

    private void ChangePassword(object obj)
    {
        int? currentUserId = RepositoryFactory.GetUserRepository().GetByNickname(SignedUser.Nickname)?.Id;
        if (currentUserId.HasValue)
        {
            this.CompleteFailed?.Invoke();
            return;
        }
        this.CompleteChangePassword?.Invoke();
        this.SignedUser.EncryptedPassword = RepositoryFactory.GetUserRepository().GetById(currentUserId!.Value)!.EncryptedPassword;
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
        this.client.Disconnect();
        this.CompleteExit?.Invoke();
    }

    private void Client_MessageReceived(Message receivedMessage)
    {
        if (receivedMessage.Type == MessageType.EndOfLine)
        {
            MessageBox.Show("Server is shut down which you were connected. Try again later.", "Server shut down",
                MessageBoxButton.OK, MessageBoxImage.Error);
            if (this.CompleteExit is not null)
                this.CompleteExit.Invoke();
            else
                Environment.Exit(0);
        }
    }
}
