﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Messenger.ViewModels;
using Messenger.Validation;
using Messenger.Models.DB;
using Messenger.Repositories;
using System.Linq;
using System.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Messenger.Views
{
    /// <summary>
    /// Interaction logic for SignInView.xaml
    /// </summary>
    public partial class SignInView : Window
    {
        public SignInView()
        {
            var servers = RepositoryFactory.GetServerRepository().GetAll();
            if (servers.IsNullOrEmpty())
            {
                this.ViewModel_CompleteCancel();
                return;
            }
            var serverNames = servers!.Select(s => s.NameServer);
            if (serverNames.IsNullOrEmpty())
            {
                this.ViewModel_CompleteCancel();
                return;
            }
            var defaultServer = ConfigurationManager.AppSettings["ServerNameByDefault"];
            if (defaultServer.IsNullOrEmpty())
            {
                this.ViewModel_CompleteCancel();
                return;
            }

            InitializeComponent();

            SignInViewModel viewModel = new();
            this.DataContext = viewModel;

            #region ViewModel Events
            viewModel.SignInCompleted += this.ViewModel_SignInCompleted;
            viewModel.CompleteSignUp += this.ViewModel_CompleteSignUp;
            viewModel.CompleteCancel += this.ViewModel_CompleteCancel;
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

            Binding lastUsingServerBinding = new(nameof(viewModel.LastUsingServer));
            lastUsingServerBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            this.comboBoxServer.SetBinding(ComboBox.TextProperty, lastUsingServerBinding);

            this.comboBoxServer.ItemsSource = serverNames;
            this.comboBoxServer.SelectedItem = defaultServer;
            #endregion

            #region ViewModel Commands
            this.buttonLogin.Command = viewModel.LoginCommand;
            this.buttonCancel.Command = viewModel.CancelCommand;
            this.buttonCreateAccount.Command = viewModel.CreateAccountCommand;
            #endregion
        }

        private void ViewModel_SignInCompleted(User signedUser)
        {
            try
            { new MainView(signedUser).Show(); }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Server is not working at the moment.", "Server shut down",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
            }
            this.Close();
        }

        private void ViewModel_CompleteSignUp()
        {
            new SignUpView().Show();
            this.Close();
        }

        private void ViewModel_CompleteCancel() =>
            this.Close();
    }
}
