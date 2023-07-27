using Messenger.Models.Application;
using Messenger.Models.DB;
using Messenger.ViewModels;
using System.Windows;

namespace Messenger.Views
{
    /// <summary>
    /// Interaction logic for VoiceRecordControlView.xaml
    /// </summary>
    public partial class VoiceRecordControlView : Window
    {
        public MultimediaMessage? FinishedMessage { get; private set; }

        public VoiceRecordControlView()
        {
            InitializeComponent();

            VoiceRecordControlViewModel viewModel = new();
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

        private void ViewModel_CompleteConfirm(MultimediaMessage message)
        {
            this.FinishedMessage = message;
            this.Close();
        }

        private void ViewModel_CompleteCancel()
        {
            this.Close();
        }
    }
}
