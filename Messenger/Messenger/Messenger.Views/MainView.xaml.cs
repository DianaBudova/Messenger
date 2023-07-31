using Messenger.Models.Application;
using Messenger.Models.DB;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Win32;
using System;
using Messenger.ViewModels.MainViewModel;
using System.Text;

namespace Messenger.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        private readonly MainViewModelContainer container;

        public MainView(User signedUser)
        {
            InitializeComponent();

            this.container = new()
            {
                MainViewModel = new(signedUser),
                UserListingViewModel = new(signedUser),
            };
            this.DataContext = container;

            #region ViewModel Events
            this.container.MainViewModel.CompleteChangeNickname += ViewModel_CompleteChangeNickname;
            this.container.MainViewModel.CompleteChangePassword += ViewModel_CompleteChangePassword;
            this.container.MainViewModel.CompleteVoiceRecord += ViewModel_CompleteVoiceRecord;
            this.container.MainViewModel.CompleteAttachFile += ViewModel_CompleteAttachFile;
            this.container.MainViewModel.CompleteChangeProfilePhoto += ViewModel_CompleteChangeProfilePhoto;
            this.container.MainViewModel.CompleteExit += ViewModel_CompleteExit;
            this.container.MainViewModel.MessageReceived += ViewModel_MessageReceived;
            #endregion

            #region ViewModel Bindings
            Binding searchedUserBinding = new(
                new StringBuilder()
                .Append(nameof(this.container.MainViewModel))
                .Append('.')
                .Append(nameof(this.container.MainViewModel.SearchedUser))
                .ToString());
            searchedUserBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            this.textBoxSearchUser.SetBinding(TextBox.TextProperty, searchedUserBinding);

            Binding inputMessageBinding = new(
                new StringBuilder()
                .Append(nameof(this.container.MainViewModel))
                .Append('.')
                .Append(nameof(this.container.MainViewModel.InputMessage))
                .ToString());
            inputMessageBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            this.textBoxYourMessage.SetBinding(TextBox.TextProperty, inputMessageBinding);

            Binding nicknameBinding = new(
                new StringBuilder()
                .Append(nameof(this.container.MainViewModel))
                .Append('.')
                .Append(nameof(this.container.MainViewModel.Nickname))
                .ToString());
            nicknameBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            this.textBoxYourNickname.SetBinding(TextBox.TextProperty, nicknameBinding);
            
            Binding profilePhotoBinding = new(
                new StringBuilder()
                .Append(nameof(this.container.MainViewModel))
                .Append('.')
                .Append(nameof(this.container.MainViewModel.ProfilePhoto))
                .ToString());
            profilePhotoBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            this.imagePhotoProfile.SetBinding(Image.SourceProperty, profilePhotoBinding);

            Binding usersBinding = new(
                new StringBuilder()
                .Append(nameof(this.container.UserListingViewModel))
                .Append('.')
                .Append(nameof(this.container.UserListingViewModel.Users))
                .ToString());
            usersBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            this.listViewUsers.SetBinding(ListView.ItemsSourceProperty, usersBinding);

            Binding messagesInChatBinding = new(
                new StringBuilder()
                .Append(nameof(this.container.MainViewModel))
                .Append('.')
                .Append(nameof(this.container.MainViewModel.Messages))
                .ToString());
            messagesInChatBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            this.listViewMessagesInChat.SetBinding(ListBox.ItemsSourceProperty, messagesInChatBinding);
            
            Binding selectedUserBinding = new(
                new StringBuilder()
                .Append(nameof(this.container.UserListingViewModel))
                .Append('.')
                .Append(nameof(this.container.UserListingViewModel.SelectedUser))
                .ToString());
            selectedUserBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            this.listViewUsers.SetBinding(ListView.SelectedItemProperty, selectedUserBinding);

            Binding selectedMessageBinding = new(
                new StringBuilder()
                .Append(nameof(this.container.MainViewModel))
                .Append('.')
                .Append(nameof(this.container.MainViewModel.SelectedMessage))
                .ToString());
            selectedMessageBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            this.listViewMessagesInChat.SetBinding(ListBox.SelectedItemProperty, selectedMessageBinding);
            #endregion

            #region ViewModel Commands
            this.buttonSearchUser.Command = this.container.MainViewModel.SearchUserCommand;
            this.buttonSendMessage.Command = this.container.MainViewModel.SendMessageCommand;
            this.buttonVoiceRecord.Command = this.container.MainViewModel.VoiceRecordCommand;
            this.buttonAttachFile.Command = this.container.MainViewModel.AttachFileCommand;
            this.buttonChangeNickname.Command = this.container.MainViewModel.ChangeNicknameCommand;
            this.buttonChangePassword.Command = this.container.MainViewModel.ChangePasswordCommand;
            this.buttonChangePhoto.Command = this.container.MainViewModel.ChangeProfilePhotoCommand;
            this.buttonClearPhoto.Command = this.container.MainViewModel.ClearProfilePhotoCommand;
            this.buttonDeleteAccount.Command = this.container.MainViewModel.DeleteAccountCommand;
            #endregion

            this.container.MainViewModel.ConnectToServer();
        }

        private void ViewModel_CompleteChangePassword() =>
            new ChangePasswordView(this.container.MainViewModel.SignedUser).ShowDialog();

        private void ViewModel_CompleteChangeNickname() =>
            new ChangeNicknameView(this.container.MainViewModel.SignedUser).ShowDialog();

        private void ViewModel_CompleteAttachFile()
        {
            AttachFileControlView view = new();
            view.ShowDialog();
            if (view.FinishedMessage is null)
                return;
            this.container.MainViewModel.MultimediaMessage = view.FinishedMessage.Value;
        }

        private void ViewModel_CompleteVoiceRecord()
        {
            VoiceRecordControlView view = new();
            view.ShowDialog();
            if (view.FinishedMessage is null)
                return;
            this.container.MainViewModel.MultimediaMessage = view.FinishedMessage.Value;
        }

        private byte[]? ViewModel_CompleteChangeProfilePhoto()
        {
            OpenFileDialog fileDialog = new();
            fileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;";
            if (fileDialog.ShowDialog() != true)
                return null;
            return System.IO.File.ReadAllBytes(fileDialog.FileName);
        }

        private void ViewModel_CompleteExit()
        {
            this.container.MainViewModel.DisconnectFromServer();
            this.Dispatcher.Invoke(this.Close);
        }

        private void ViewModel_MessageReceived(Message receivedMessage)
        {
            Message message = new(receivedMessage);
        }

        protected override void OnClosed(EventArgs e)
        {
            this.container.MainViewModel.DisconnectFromServer();
            this.Dispatcher.Invoke(this.Close);
        }
    }
}
