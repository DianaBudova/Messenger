using Messenger.Models.Application;
using Messenger.ViewModels;
using System.Windows;

namespace Messenger.Views
{
    /// <summary>
    /// Interaction logic for VoiceRecordControlView.xaml
    /// </summary>
    public partial class VoiceRecordControlView : Window
    {
        public Message? ObtainedMessage { get; private set; }

        public VoiceRecordControlView(User currentUser)
        {
            InitializeComponent();

            VoiceRecordControlViewModel viewModel = new(currentUser);
            this.DataContext = viewModel;

            #region ViewModel Events
            viewModel.CompleteConfirm += ViewModel_CompleteConfirm;
            viewModel.CompleteCancel += ViewModel_CompleteCancel;
            #endregion

            #region ViewModel Bindings
            #endregion

            #region ViewModel Commands
            this.buttonStartRecordingVoice.Command = viewModel.StartRecordingVoiceCommand;
            this.buttonPauseRecordingVoice.Command = viewModel.PauseRecordingVoiceCommand;
            this.buttonStopRecordingVoice.Command = viewModel.StopRecordingVoiceCommand;
            this.buttonStartListeningVoiceMessage.Command = viewModel.StartListeningVoiceMessageCommand;
            this.buttonStopListeningVoiceMessage.Command= viewModel.StopListeningVoiceMessageCommand;
            this.buttonConfirm.Command = viewModel.ConfirmCommand;
            this.buttonCancel.Command = viewModel.CancelCommand;
            #endregion
        }

        private void ViewModel_CompleteConfirm(Message message)
        {
            this.ObtainedMessage = message;
            this.Close();
        }

        private void ViewModel_CompleteCancel()
        {
            this.Close();
        }
    }
}
