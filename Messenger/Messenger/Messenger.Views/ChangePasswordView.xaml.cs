using Messenger.Models.DB;
using Messenger.Validation;
using Messenger.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Messenger.Views
{
    /// <summary>
    /// Interaction logic for ChangePasswordView.xaml
    /// </summary>
    public partial class ChangePasswordView : Window
    {
        public ChangePasswordView(User currentUser)
        {
            InitializeComponent();

            ChangePasswordViewModel viewModel = new(currentUser);
            this.DataContext = viewModel;

            #region ViewModel Events
            viewModel.ConfirmCompleted += this.ViewModel_ConfirmCompleted;
            viewModel.ConfirmFailed += this.ViewModel_ConfirmFailed;
            viewModel.CompleteCancel += this.ViewModel_CompleteCancel;
            #endregion

            #region ViewModel Bindings
            Binding newPasswordBinding = new(nameof(viewModel.NewPassword));
            newPasswordBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            newPasswordBinding.ValidationRules.Add(new PasswordValidationRule());
            this.textBoxNewPassword.SetBinding(TextBox.TextProperty, newPasswordBinding);
            #endregion

            #region ViewModel Commands
            this.buttonConfirm.Command = viewModel.ConfirmCommand;
            this.buttonCancel.Command = viewModel.CancelCommand;
            #endregion
        }

        private void ViewModel_ConfirmCompleted()
        {
            MessageBox.Show("Password has been changed successfully!", "",
                MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void ViewModel_ConfirmFailed()
        {
            MessageBox.Show("Something went wrong.", "",
                MessageBoxButton.OK, MessageBoxImage.Error);
            this.Close();
        }

        private void ViewModel_CompleteCancel() =>
            this.Close();
    }
}
