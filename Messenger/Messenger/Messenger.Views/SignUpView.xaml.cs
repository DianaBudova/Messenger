using Messenger.Models.DB;
using Messenger.Repositories;
using Messenger.Validation;
using Messenger.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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
            viewModel.SignUpCompleted += ViewModel_SignUpCompleted;
            viewModel.CompleteCancel += ViewModel_CompleteCancel;
            viewModel.CompleteSignIn += ViewModel_CompleteSignIn;
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
            List<Server>? servers = RepositoryFactory.GetServerRepository().GetAll();
            if (servers is not null ||
                servers!.Select(s => s.NameServer).Contains(ConfigurationManager.AppSettings["ServerNameByDefault"]))
                new SignInView(servers!).Show();
            else
                MessageBox.Show("There are no working servers at the moment. Try again later.", "Servers are not working",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void ViewModel_SignUpCompleted()
        {
            List<Server>? servers = RepositoryFactory.GetServerRepository().GetAll();
            if (servers is not null ||
                servers!.Select(s => s.NameServer).Contains(ConfigurationManager.AppSettings["ServerNameByDefault"]))
                new SignInView(servers!).Show();
            else
                MessageBox.Show("There are no working servers at the moment. Try again later.", "Servers are not working",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void ViewModel_CompleteCancel()
        {
            Environment.Exit(0);
        }
    }
}
