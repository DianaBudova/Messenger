using Messenger.Models.Application;
using Messenger.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Win32;
using System.IO;

namespace Messenger.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        private readonly MainViewModel viewModel;

        public MainView(User signedUser)
        {
            InitializeComponent();

            this.viewModel = new(signedUser);
            this.DataContext = viewModel;

            #region ViewModel Events
            viewModel.CompleteChangeNickname += ViewModel_CompleteChangeNickname;
            viewModel.CompleteChangePassword += ViewModel_CompleteChangePassword;
            viewModel.CompleteVoiceRecord += ViewModel_CompleteVoiceRecord;
            viewModel.CompleteAttachFile += ViewModel_CompleteAttachFile;
            viewModel.CompleteChangeProfilePhoto += ViewModel_CompleteChangeProfilePhoto;
            #endregion

            #region ViewModel Bindings
            Binding searchedUserBinding = new(nameof(viewModel.SearchedUser));
            searchedUserBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            this.textBoxSearchUser.SetBinding(TextBox.TextProperty, searchedUserBinding);

            Binding inputMessageBinding = new(nameof(viewModel.InputMessage));
            inputMessageBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            this.textBoxYourMessage.SetBinding(TextBox.TextProperty, inputMessageBinding);

            Binding nicknameBinding = new(nameof(viewModel.Nickname));
            nicknameBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            this.textBoxYourNickname.SetBinding(TextBox.TextProperty, nicknameBinding);
            
            Binding profilePhotoBinding = new(nameof(viewModel.ProfilePhoto));
            profilePhotoBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            this.imagePhotoProfile.SetBinding(Image.SourceProperty, profilePhotoBinding);

            Binding usersBinding = new(nameof(viewModel.Users));
            usersBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            this.listViewUsers.SetBinding(ListView.ItemsSourceProperty, usersBinding);

            Binding messagesInChatBinding = new(nameof(viewModel.Messages));
            messagesInChatBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            this.listBoxMessagesInChat.SetBinding(ListBox.ItemsSourceProperty, messagesInChatBinding);

            Binding selectedUserBinding = new(nameof(viewModel.SelectedUser));
            selectedUserBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            this.listViewUsers.SetBinding(ListView.SelectedItemProperty, selectedUserBinding);

            Binding selectedMessageBinding = new(nameof(viewModel.SelectedMessage));
            selectedMessageBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            this.listBoxMessagesInChat.SetBinding(ListBox.SelectedItemProperty, selectedMessageBinding);
            #endregion

            #region ViewModel Commands
            this.buttonSearchUser.Command = viewModel.SearchUserCommand;
            this.buttonSendMessage.Command = viewModel.SendMessageCommand;
            this.buttonVoiceRecord.Command = viewModel.VoiceRecordCommand;
            this.buttonAttachFile.Command = viewModel.AttachFileCommand;
            this.buttonChangeNickname.Command = viewModel.ChangeNicknameCommand;
            this.buttonChangePassword.Command = viewModel.ChangePasswordCommand;
            this.buttonChangePhoto.Command = viewModel.ChangeProfilePhotoCommand;
            this.buttonClearPhoto.Command = viewModel.ClearProfilePhotoCommand;
            #endregion
        }

        private void ViewModel_CompleteChangePassword()
        {
            new ChangePasswordView(this.viewModel.SignedUser).ShowDialog();
        }

        private void ViewModel_CompleteChangeNickname()
        {
            new ChangeNicknameView(this.viewModel.SignedUser).ShowDialog();
        }

        private void ViewModel_CompleteAttachFile()
        {
            new AttachFileControlView().ShowDialog();
        }

        private void ViewModel_CompleteVoiceRecord()
        {
            new VoiceRecordControlView().ShowDialog();
        }

        private byte[]? ViewModel_CompleteChangeProfilePhoto()
        {
            OpenFileDialog fileDialog = new();
            fileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;";
            if (fileDialog.ShowDialog() != true)
                return null;
            return System.IO.File.ReadAllBytes(fileDialog.FileName);
        }
    }
}
