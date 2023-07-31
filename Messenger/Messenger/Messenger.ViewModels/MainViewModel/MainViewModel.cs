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
using System.Threading;

namespace Messenger.ViewModels.MainViewModel;

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
        { return searchedUser; }
        set
        {
            searchedUser = value;
            OnPropertyChanged();
        }
    }
    private string? inputMessage;
    public string? InputMessage
    {
        get
        { return inputMessage; }
        set
        {
            inputMessage = value;
            OnPropertyChanged();
        }
    }
    private string? nickname;
    public string? Nickname
    {
        get
        { return nickname; }
        set
        {
            nickname = value;
            OnPropertyChanged();
        }
    }
    private byte[]? profilePhoto;
    public byte[]? ProfilePhoto
    {
        get
        { return profilePhoto; }
        set
        {
            profilePhoto = value;
            OnPropertyChanged();
        }
    }
    private User? selectedUser;
    public User? SelectedUser
    {
        get
        { return selectedUser; }
        set
        {
            selectedUser = value;
            OnPropertyChanged();
            Application.Current.Dispatcher.Invoke(Messages.Clear);
            if (SelectedUser is not null)
            {
                var sentMessages = RepositoryFactory.GetChatRepository().GetBySenderRecipientId(SignedUser.Id, SelectedUser.Id);
                var receivedMessages = RepositoryFactory.GetChatRepository().GetBySenderRecipientId(SelectedUser.Id, SignedUser.Id);
                if (sentMessages is null || receivedMessages is null)
                    return;
                var allMessages = sentMessages.Concat(receivedMessages).OrderBy(item => item.DateTime).ToList();
                foreach (var message in allMessages)
                    Application.Current.Dispatcher.Invoke(() => Messages.Add(message));
            }
        }
    }
    private Chat? selectedMessage;
    public Chat? SelectedMessage
    {
        get
        { return selectedMessage; }
        set
        {
            selectedMessage = value;
            OnPropertyChanged();
        }
    }
    private MultimediaMessage? messageToSend;
    public MultimediaMessage? MultimediaMessage
    {
        get
        { return messageToSend; }
        set
        {
            messageToSend = value;
            OnPropertyChanged();
        }
    }
    private ObservableCollection<User>? tempUsers;
    private ObservableCollection<Chat>? messages;
    public ObservableCollection<Chat> Messages
    {
        get
        { return messages; }
        set
        {
            messages = value;
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
        SearchUserCommand = new(SearchUser);
        SendMessageCommand = new(SendMessage);
        VoiceRecordCommand = new(VoiceRecord);
        AttachFileCommand = new(AttachFile);
        ChangeNicknameCommand = new(ChangeNickname);
        ChangePasswordCommand = new(ChangePassword);
        ChangeProfilePhotoCommand = new(ChangeProfilePhoto);
        ClearProfilePhotoCommand = new(ClearProfilePhoto);
        DeleteAccountCommand = new(DeleteAccount);
        #endregion

        SignedUser = signedUser;
        Nickname = signedUser.Nickname;
        ProfilePhoto = signedUser.ProfilePhoto;
        Messages = new();
        recorder = new();
        try
        {
            string? serverName = SignedUser.LastUsingServer?.NameServer ?? ConfigurationManager.AppSettings["ServerNameByDefault"];
            if (serverName is null)
                throw new Exception();
            Server? chosenServer = RepositoryFactory.GetServerRepository().GetByNameServer(serverName)
                ?? throw new Exception();
            IPEndPoint ep = new(IPAddress.Parse(chosenServer.IpAddress), chosenServer.Port);
            client = new(ep);
        }
        catch
        {
            Environment.Exit(0);
        }
        client.MessageReceived += Client_MessageReceived;
    }

    public User ConnectToServer()
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
            this.SignedUser.Port = existedUser.Port = port;
            RepositoryFactory.GetUserRepository().Update(existedUser);
        }
        catch
        {
            if (CompleteExit is not null)
                CompleteExit.Invoke();
            else
                Environment.Exit(0);
        }
        return this.SignedUser;
    }

    public void DisconnectFromServer()
    {
        try
        {
            client.Disconnect();
            User? existedUser = RepositoryFactory.GetUserRepository().GetByNickname(SignedUser.Nickname);
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
        //try
        //{
        //    if (SearchedUser.IsNullOrEmpty())
        //    {
        //        if (tempUsers is not null || tempUsers?.Count > 0)
        //        {
        //            Users = new(tempUsers);
        //            tempUsers.Clear();
        //        }
        //        return;
        //    }
        //    var allUsers = RepositoryFactory.GetUserRepository().GetAll();
        //    if (allUsers is null)
        //        return;
        //    allUsers.Remove(allUsers.Find(user => SignedUser.IsSimilar(user)));
        //    if (tempUsers is null || tempUsers.Count == 0)
        //        tempUsers = new(Users);
        //    List<User> searchedUsers = new();
        //    foreach (var user in allUsers)
        //        if (user.Nickname.Contains(SearchedUser!))
        //            searchedUsers.Add(user);
        //    Users.Clear();
        //    Users = new(searchedUsers);
        //}
        //catch
        //{
        //    MessageBox.Show("Some error occured.", "",
        //        MessageBoxButton.OK, MessageBoxImage.Error);
        //    return;
        //}
    }

    private void SendMessage(object obj)
    {
        if (SelectedUser is null)
            return;
        Message messageToSend = new()
        {
            Sender = SignedUser,
            Recipient = SelectedUser,
            DateTime = DateTime.Now,
        };
        try
        {
            if (MultimediaMessage is not null && !MultimediaMessage.Value.Content.IsNullOrEmpty())
            {
                messageToSend.Content = MultimediaMessage.Value.Content;
                messageToSend.Type = MultimediaMessage.Value.Type == MultimediaMessageType.File ? MessageType.File : MessageType.Audio;
            }
            else if (!InputMessage.IsNullOrEmpty())
            {
                messageToSend.Content = JsonSerializer.SerializeToUtf8Bytes(InputMessage);
                messageToSend.Type = MessageType.Text;
            }
            else
                throw new Exception();
            client.SendMessage(messageToSend);
        }
        catch
        {
            MessageBox.Show("Server is not working at the moment or there is no message to send.", "Error occurred when sending a message",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void VoiceRecord(object obj)
    {
        if (!recorder.CanStartRecording())
            return;
        if (MessageBox.Show("Click OK to start recording voice message.", "Start recording voice message?",
            MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
            CompleteVoiceRecord?.Invoke();
    }

    private void AttachFile(object obj)
    {
        CompleteAttachFile?.Invoke();
    }

    private void ChangeNickname(object obj)
    {
        int currentUserId = RepositoryFactory.GetUserRepository().GetByNickname(SignedUser.Nickname).Id;
        CompleteChangeNickname?.Invoke();
        SignedUser.Nickname = RepositoryFactory.GetUserRepository().GetById(currentUserId)!.Nickname;
        Nickname = SignedUser.Nickname;
    }

    private void ChangePassword(object obj)
    {
        int currentUserId = RepositoryFactory.GetUserRepository().GetByNickname(SignedUser.Nickname).Id;
        CompleteChangePassword?.Invoke();
        SignedUser.EncryptedPassword = RepositoryFactory.GetUserRepository().GetById(currentUserId)!.EncryptedPassword;
    }

    private void ChangeProfilePhoto(object obj)
    {
        byte[]? image = CompleteChangeProfilePhoto?.Invoke();
        if (image is null)
            return;
        SignedUser.ProfilePhoto = image;
        ProfilePhoto = image;
        var updatedUser = RepositoryFactory.GetUserRepository().GetByNickname(SignedUser.Nickname);
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
        SignedUser.ProfilePhoto = System.IO.File.ReadAllBytes(ConfigurationManager.AppSettings["ImagesPath"] + "UnknownUser.png");
        ProfilePhoto = System.IO.File.ReadAllBytes(ConfigurationManager.AppSettings["ImagesPath"] + "UnknownUser.png");
        var updatedUser = RepositoryFactory.GetUserRepository().GetByNickname(SignedUser.Nickname);
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
        var deletedUser = RepositoryFactory.GetUserRepository().GetByNickname(SignedUser.Nickname);
        if (deletedUser is null)
        {
            MessageBox.Show("Such a user does not exist in the database.", "",
                MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        RepositoryFactory.GetUserRepository().Remove(deletedUser);
        MessageBox.Show("User was deleted successfully!", "",
            MessageBoxButton.OK, MessageBoxImage.Information);
        RepositoryFactory.GetUserRepository().Update(new(RepositoryFactory.GetUserRepository().GetByNickname(SignedUser.Nickname)) { Port = null });
        client.Disconnect();
        CompleteExit?.Invoke();
    }

    private void Client_MessageReceived(Message receivedMessage)
    {
        if (receivedMessage.Type != MessageType.EndOfLine)
            MessageReceived?.Invoke(receivedMessage);
        else
        {
            MessageBox.Show("Server is shut down which you were connected. Try again later.", "Server shut down",
                MessageBoxButton.OK, MessageBoxImage.Information);
            if (CompleteExit is not null)
                CompleteExit.Invoke();
            else
                Environment.Exit(0);
        }
    }
}
