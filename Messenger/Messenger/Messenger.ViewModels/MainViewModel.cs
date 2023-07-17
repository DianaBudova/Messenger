using Messenger.BL;
using Messenger.Models.Application;
using System;
using System.Windows;
using System.Collections.ObjectModel;
using Messenger.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Configuration;

namespace Messenger.ViewModels;

public class MainViewModel : ViewModelBase
{
    public event Action? CompleteChangeNickname;
    public event Action? CompleteChangePassword;
    public event Action? CompleteVoiceRecord;
    public event Action? CompleteAttachFile;
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
    private string? inputMessage;
    private string? nickname;
    private byte[]? profilePhoto;
    private User? selectedUser;
    private Chat? selectedMessage;
    public string SearchedUser
    {
        get
        { return this.searchedUser; }
        set
        {
            this.searchedUser = value;
            this.OnPropertyChanged();
        }
    }
    public string InputMessage
    {
        get
        { return this.inputMessage; }
        set
        {
            this.inputMessage = value;
            this.OnPropertyChanged();
        }
    }
    public string Nickname
    {
        get
        { return this.nickname; }
        set
        {
            this.nickname = value;
            this.OnPropertyChanged();
        }
    }
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
    public User SelectedUser
    {
        get
        { return this.selectedUser; }
        set
        {
            this.selectedUser = value;
            OnPropertyChanged();
        }
    }
    public Chat SelectedMessage
    {
        get
        { return this.selectedMessage; }
        set
        {
            this.selectedMessage = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<User>? users;
    private ObservableCollection<User>? tempUsers;
    private ObservableCollection<Chat>? messages;
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
    public Message? MessageToSend { get; set; }
    private readonly VoiceRecorderAdapter recorder;
    private readonly TcpSocket socket;

    public MainViewModel(User signedUser)
    {
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
        var existedUsers = RepositoryFactory.GetUserRepository().GetAll();
        foreach (var user in existedUsers!)
        {
            try
            {
                if (this.SignedUser.IsSimilar(User.Parse(user)))
                    continue;
                this.Users?.Add(new()
                {
                    Nickname = user.Nickname,
                    EncryptedPassword = user.EncryptedPassword,
                    ProfilePhoto = user.ProfilePhoto,
                });
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Some error occured.", "", 
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                Application.Current.Shutdown();
            }
        }
        this.recorder = new();
        //this.socket = new();
        //var currentUser = RepositoryFactory.GetUserRepository().GetByNickname(this.Nickname);
        //if (currentUser is null)
        //    return;
        //this.socket.Connect(currentUser.IpAddress, int.Parse(currentUser.Port));
        //this.socket.MessageReceived += Socket_MessageReceived;
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
            allUsers.Remove(allUsers.Find(user => this.SignedUser.IsSimilar(User.Parse(user))));
            if (this.tempUsers is null || this.tempUsers.Count == 0)
                this.tempUsers = new(this.Users);
            List<User> searchedUsers = new();
            foreach (var user in allUsers)
                if (user.Nickname.Contains(this.SearchedUser!))
                    searchedUsers.Add(User.Parse(user));
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
        //this.MessageToSend;
    }

    private void VoiceRecord(object obj)
    {
        if (this.recorder.State is VoiceRecorderState.None ||
            this.recorder.State is VoiceRecorderState.NonRecords)
        {
            if (MessageBox.Show("Click OK to start recording voice message.", "Start recording voice message?",
                MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
                this.CompleteVoiceRecord?.Invoke();
        }
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
        Application.Current.Shutdown();
    }
}
