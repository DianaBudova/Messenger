using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Messenger.Models.Application;
using Messenger.Models.DB;
using Messenger.ViewModels;
using Microsoft.Win32;

namespace Messenger.Views
{
    /// <summary>
    /// Interaction logic for AttachFileControlView.xaml
    /// </summary>
    public partial class AttachFileControlView : Window
    {
        public MultimediaMessage? FinishedMessage { get; private set; }

        public AttachFileControlView()
        {
            InitializeComponent();

            AttachFileControlViewModel viewModel = new();
            this.DataContext = viewModel;

            #region ViewModel Events
            viewModel.CompleteAttachment += ViewModel_CompleteAttachment;
            viewModel.CompleteConfirm += ViewModel_CompleteConfirm;
            viewModel.CompleteCancel += ViewModel_CompleteCancel;
            #endregion

            #region ViewModel Bindings
            Binding pathBinding = new(nameof(viewModel.Path));
            pathBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            this.textBoxPath.SetBinding(TextBox.TextProperty, pathBinding);

            Binding sizeBinding = new(nameof(viewModel.Size));
            sizeBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            this.textBoxSize.SetBinding(TextBox.TextProperty, sizeBinding);
            #endregion

            #region ViewModel Commands
            this.buttonAttachFile.Command = viewModel.AttachFileCommand;
            this.buttonConfirm.Command = viewModel.ConfirmCommand;
            this.buttonCancel.Command = viewModel.CancelCommand;
            #endregion
        }

        private Models.Application.File? ViewModel_CompleteAttachment()
        {
            OpenFileDialog fileDialog = new();
            if (fileDialog.ShowDialog() != true)
                return null;
            return new Models.Application.File
            {
                Path = fileDialog.FileName,
                Content = System.IO.File.ReadAllBytes(fileDialog.FileName),
                SizeBytes = new FileInfo(fileDialog.FileName).Length,
            };
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
