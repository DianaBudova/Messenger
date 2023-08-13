using Messenger.Models.Application;
using Messenger.ViewModels;
using System;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

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
            viewModel.CompleteConfirm += this.ViewModel_CompleteConfirm;
            viewModel.CompleteCancel += this.ViewModel_CompleteCancel;
            #endregion

            #region ViewModel Bindings
            Binding isRecordingBinding = new(nameof(viewModel.IsRecording));
            isRecordingBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            Style style = new();
            DataTrigger isRecordingTrue = new()
            {
                Binding = isRecordingBinding,
                Value = true,
            };
            isRecordingTrue.Setters.Add(new Setter()
            {
                Property = Image.SourceProperty,
                Value = new BitmapImage(new Uri($"{ConfigurationManager.AppSettings["ImagesPath"]}RedCircle.png", UriKind.Relative)),
            });
            DataTrigger isRecordingFalse = new()
            {
                Binding = isRecordingBinding,
                Value = false,
            };
            isRecordingFalse.Setters.Add(new Setter()
            {
                Property = Image.SourceProperty,
                Value = new BitmapImage(new Uri($"{ConfigurationManager.AppSettings["ImagesPath"]}GreyCircle.png", UriKind.Relative)),
            });
            style.Triggers.Add(isRecordingTrue);
            style.Triggers.Add(isRecordingFalse);
            this.imageStatus.Style = style;
            #endregion

            #region ViewModel Commands
            this.buttonStartRecordingVoice.Command = viewModel.StartRecordingVoiceCommand;
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

        private void ViewModel_CompleteCancel() =>
            this.Close();
    }
}
