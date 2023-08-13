using Messenger.Models.DB;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Win32;
using System;
using Messenger.ViewModels;
using Messenger.Models.Application;
using Messenger.Common;
using Newtonsoft.Json.Linq;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
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
            this.viewModel.CompleteChangeNickname += this.ViewModel_CompleteChangeNickname;
            this.viewModel.CompleteChangePassword += this.ViewModel_CompleteChangePassword;
            this.viewModel.CompleteVoiceRecord += this.ViewModel_CompleteVoiceRecord;
            this.viewModel.CompleteAttachFile += this.ViewModel_CompleteAttachFile;
            this.viewModel.CompleteChangeProfilePhoto += this.ViewModel_CompleteChangeProfilePhoto;
            this.viewModel.MessageSendCompleted += this.ViewModel_MessageSendCompleted;
            this.viewModel.CompleteFailed += this.ViewModel_CompleteFailed;
            this.viewModel.CompleteExit += this.ViewModel_CompleteExit;
            #endregion

            #region ViewModel Bindings
            Binding searchedUserBinding = new(nameof(this.viewModel.SearchedUser));
            searchedUserBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            this.textBoxSearchUser.SetBinding(TextBox.TextProperty, searchedUserBinding);

            Binding inputMessageBinding = new(nameof(this.viewModel.InputMessage));
            inputMessageBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            this.textBoxYourMessage.SetBinding(TextBox.TextProperty, inputMessageBinding);

            Binding nicknameBinding = new(nameof(this.viewModel.Nickname));
            nicknameBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            this.textBoxYourNickname.SetBinding(TextBox.TextProperty, nicknameBinding);
            
            Binding profilePhotoBinding = new(nameof(this.viewModel.ProfilePhoto));
            profilePhotoBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            this.imagePhotoProfile.SetBinding(Image.SourceProperty, profilePhotoBinding);

            Binding usersBinding = new(nameof(this.viewModel.Users));
            usersBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            this.listViewUsers.SetBinding(ListView.ItemsSourceProperty, usersBinding);

            Binding messagesInChatBinding = new(nameof(this.viewModel.Messages));
            messagesInChatBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            this.listViewMessagesInChat.SetBinding(ListBox.ItemsSourceProperty, messagesInChatBinding);
            
            Binding selectedUserBinding = new(nameof(this.viewModel.SelectedUser));
            selectedUserBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            this.listViewUsers.SetBinding(ListView.SelectedItemProperty, selectedUserBinding);

            Binding selectedMessageBinding = new(nameof(this.viewModel.SelectedMessage));
            selectedMessageBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            this.listViewMessagesInChat.SetBinding(ListBox.SelectedItemProperty, selectedMessageBinding);
            #endregion

            #region ViewModel Commands
            this.buttonSendMessage.Command = this.viewModel.SendMessageCommand;
            this.buttonVoiceRecord.Command = this.viewModel.VoiceRecordCommand;
            this.buttonAttachFile.Command = this.viewModel.AttachFileCommand;
            this.buttonChangeNickname.Command = this.viewModel.ChangeNicknameCommand;
            this.buttonChangePassword.Command = this.viewModel.ChangePasswordCommand;
            this.buttonChangePhoto.Command = this.viewModel.ChangeProfilePhotoCommand;
            this.buttonClearPhoto.Command = this.viewModel.ClearProfilePhotoCommand;
            this.buttonDeleteAccount.Command = this.viewModel.DeleteAccountCommand;
            #endregion

            this.viewModel.ConnectToServer();
        }

        private void ViewModel_CompleteChangePassword() =>
            new ChangePasswordView(this.viewModel.SignedUser).ShowDialog();

        private void ViewModel_CompleteChangeNickname() =>
            new ChangeNicknameView(this.viewModel.SignedUser).ShowDialog();

        private void ViewModel_CompleteAttachFile()
        {
            AttachFileControlView view = new();
            view.ShowDialog();
            if (view.FinishedMessage is null)
                return;
            this.viewModel.MultimediaMessage = view.FinishedMessage.Value;
        }

        private void ViewModel_CompleteVoiceRecord()
        {
            VoiceRecordControlView view = new();
            view.ShowDialog();
            if (view.FinishedMessage is null)
                return;
            this.viewModel.MultimediaMessage = view.FinishedMessage.Value;
        }

        private byte[]? ViewModel_CompleteChangeProfilePhoto()
        {
            OpenFileDialog fileDialog = new();
            fileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;";
            if (fileDialog.ShowDialog() != true)
                return null;
            return System.IO.File.ReadAllBytes(fileDialog.FileName);
        }

        private void ViewModel_MessageSendCompleted() =>
            this.textBoxYourMessage.Text = string.Empty;

        private void ViewModel_CompleteFailed()
        {
            MessageBox.Show("Something went wrong.", "",
                MessageBoxButton.OK, MessageBoxImage.Error);
            this.Close();
        }

        private void ViewModel_CompleteExit()
        {
            this.viewModel.DisconnectFromServer();
            this.Dispatcher.Invoke(this.Close);
        }

        protected override void OnClosed(EventArgs e)
        {
            this.viewModel.DisconnectFromServer();
            this.Dispatcher.Invoke(this.Close);
        }

        private void listViewMessagesInChat_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is ListView listView)
            {
                object selectedItem = listView.SelectedItem;
                if (selectedItem is Chat message)
                {
                    if (message.MessageType == MessageType.File)
                    {
                        string fileAsJson = Encoding.UTF8.GetString(message.Message);
                        byte[]? content = JObject.Parse(fileAsJson)["Content"]?.ToObject<byte[]>();
                        if (content.IsNullOrEmpty())
                            return;
                        string? path = JObject.Parse(fileAsJson)["Path"]?.ToString();
                        if (path.IsNullOrEmpty())
                            return;
                        string? fileName = Path.GetFileName(path);
                        if (fileName.IsNullOrEmpty())
                            return;
                        string downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads" + $"\\{fileName}";
                        System.IO.File.WriteAllBytes(downloadsPath, content!);
                        MessageBox.Show("File from message successfully saved!", "File saved",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
        }
    }
}
