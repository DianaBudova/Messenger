using Messenger.Validation;
using Messenger.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Messenger.Views
{
    /// <summary>
    /// Interaction logic for SignUpView.xaml
    /// </summary>
    public partial class SignUpView : Window
    {
        public SignUpView()
        {
            InitializeComponent();

            SignUpViewModel viewModel = new();
            this.DataContext = viewModel;

            #region ViewModel Events
            viewModel.SignUpCompleted += this.ViewModel_SignUpCompleted;
            viewModel.CompleteCancel += this.ViewModel_CompleteCancel;
            viewModel.CompleteSignIn += this.ViewModel_CompleteSignIn;
            #endregion

            #region ViewModel Bindings
            Binding newNicknameBinding = new(nameof(viewModel.NewNickname));
            newNicknameBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            newNicknameBinding.ValidationRules.Add(new SignUpNicknameValidationRule());
            this.textBoxInputNewNickname.SetBinding(TextBox.TextProperty, newNicknameBinding);

            Binding newPasswordBinding = new(nameof(viewModel.NewPassword));
            newPasswordBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            newPasswordBinding.ValidationRules.Add(new PasswordValidationRule());
            this.textBoxInputNewPassword.SetBinding(TextBox.TextProperty, newPasswordBinding);

            Binding repeatedPasswordBinding = new(nameof(viewModel.RepeatedPassword));
            repeatedPasswordBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            repeatedPasswordBinding.ValidationRules.Add(new PasswordValidationRule());
            this.textBoxInputRepeatedPassword.SetBinding(TextBox.TextProperty, repeatedPasswordBinding);
            #endregion

            #region ViewModel Commands
            this.buttonCreate.Command = viewModel.CreateAccountCommand;
            this.buttonCancel.Command = viewModel.CancelCommand;
            this.buttonSignIn.Command = viewModel.SignInCommand;
            #endregion
        }

        private void ViewModel_CompleteSignIn()
        {
            new SignInView().Show();
            this.Close();
        }

        private void ViewModel_SignUpCompleted()
        {
            new SignInView().Show();
            this.Close();
        }

        private void ViewModel_CompleteCancel() =>
            this.Close();
    }
}
