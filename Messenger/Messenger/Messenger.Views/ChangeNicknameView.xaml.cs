using Messenger.Models.DB;
using Messenger.Validation;
using Messenger.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Messenger.Views
{
    /// <summary>
    /// Interaction logic for ChangeNicknameView.xaml
    /// </summary>
    public partial class ChangeNicknameView : Window
    {
        public ChangeNicknameView(User currentUser)
        {
            InitializeComponent();

            ChangeNicknameViewModel viewModel = new(currentUser);
            this.DataContext = viewModel;

            #region ViewModel Events
            viewModel.ConfirmCompleted += ViewModel_ConfirmCompleted;
            viewModel.CompleteCancel += ViewModel_CompleteCancel;
            #endregion

            #region ViewModel Bindings
            Binding newNicknameBinding = new(nameof(viewModel.NewNickname));
            newNicknameBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            newNicknameBinding.ValidationRules.Add(new SignUpNicknameValidationRule());
            this.textBoxNewNickname.SetBinding(TextBox.TextProperty, newNicknameBinding);

            Binding oldNicknameBinding = new(nameof(viewModel.OldNickname));
            oldNicknameBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            oldNicknameBinding.ValidationRules.Add(new SignInNicknameValidationRule());
            this.textBoxOldNickname.SetBinding(TextBox.TextProperty, oldNicknameBinding);
            #endregion

            #region ViewModel Commands
            this.buttonConfirm.Command = viewModel.ConfirmCommand;
            this.buttonCancel.Command = viewModel.CancelCommand;
            #endregion
        }

        private void ViewModel_ConfirmCompleted()
        {
            MessageBox.Show("Nickname has been changed successfully!", "",
                MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void ViewModel_CompleteCancel()
        {
            this.Close();
        }
    }
}
