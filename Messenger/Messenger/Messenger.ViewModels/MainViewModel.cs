using Messenger.BL;
using Messenger.Models.Application;
using System;
using System.Windows;
using System.Collections.ObjectModel;
using Messenger.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Linq;

namespace Messenger.ViewModels;

public class MainViewModel : ViewModelBase
{
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

    private readonly User signedUser;
    private readonly VoiceRecorderAdapter recorder;

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
        #endregion

        this.signedUser = signedUser;
        this.Nickname = signedUser.Nickname;
        this.ProfilePhoto = signedUser.ProfilePhoto;
        this.Users = new();
        this.Messages = new();
        var existedUsers = RepositoryFactory.GetUserRepository().GetAll();
        foreach (var user in existedUsers!)
        {
            try
            {
                if (this.signedUser.IsSimilar(User.Parse(user)))
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
            allUsers.Remove(allUsers.Find(user => this.signedUser.IsSimilar(User.Parse(user))));
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

    }

    private void ChangePassword(object obj)
    {

    }

    private void ChangeProfilePhoto(object obj)
    {
        byte[]? image = this.CompleteChangeProfilePhoto?.Invoke();
        if (image is null)
            return;
        Models.DB.User? updatedUser = RepositoryFactory.GetUserRepository().GetByNickname(this.signedUser.Nickname);
        if (updatedUser is null)
            return;
        updatedUser.ProfilePhoto = image;
        RepositoryFactory.GetUserRepository().Update(updatedUser);
        this.signedUser.ProfilePhoto = image;
        this.ProfilePhoto = image;
    }
}
