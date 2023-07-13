using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Messenger.ViewModels;
using Messenger.Validation;
using Messenger.Models.Application;
using Messenger.Repositories;
using Messenger.Cryptography;

namespace Messenger.Views
{
    /// <summary>
    /// Interaction logic for SignInView.xaml
    /// </summary>
    public partial class SignInView : Window
    {
        public SignInView()
        {
            InitializeComponent();

            SignInViewModel viewModel = new();
            this.DataContext = viewModel;

            #region ViewModel Events
            viewModel.SignInCompleted += ViewModel_SignInCompleted;
            viewModel.CompleteSignUp += ViewModel_CompleteSignUp;
            viewModel.CompleteCancel += ViewModel_CompleteCancel;
            #endregion

            #region ViewModel Bindings
            Binding nicknameBinding = new(nameof(viewModel.Nickname));
            nicknameBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            nicknameBinding.ValidationRules.Add(new SignInNicknameValidationRule());
            this.textBoxInputNickname.SetBinding(TextBox.TextProperty, nicknameBinding);

            Binding passwordBinding = new(nameof(viewModel.Password));
            passwordBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            passwordBinding.ValidationRules.Add(new PasswordValidationRule());
            this.textBoxInputPassword.SetBinding(TextBox.TextProperty, passwordBinding);
            #endregion

            #region ViewModel Commands
            this.buttonLogin.Command = viewModel.LoginCommand;
            this.buttonCancel.Command = viewModel.CancelCommand;
            this.buttonCreateAccount.Command = viewModel.CreateAccountCommand;
            #endregion
        }

        private void ViewModel_SignInCompleted()
        {
            User signedUser = new()
            {
                Nickname = this.textBoxInputNickname.Text,
                EncryptedPassword = HashData.EncryptData(this.textBoxInputPassword.Text),
                ProfilePhoto = RepositoryFactory.GetUserRepository().GetByNickname(this.textBoxInputNickname.Text).ProfilePhoto,
            };
            new MainView(signedUser).Show();
            this.Close();
        }

        private void ViewModel_CompleteSignUp()
        {
            new SignUpView().Show();
            this.Close();
        }

        private void ViewModel_CompleteCancel()
        {
            this.Close();
        }
    }
}
