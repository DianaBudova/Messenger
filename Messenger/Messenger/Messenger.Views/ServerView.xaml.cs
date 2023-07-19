using Messenger.ViewModels;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Messenger.Views
{
    /// <summary>
    /// Interaction logic for ServerView.xaml
    /// </summary>
    public partial class ServerView : Window
    {
        public ServerView()
        {
            InitializeComponent();

            ServerViewModel viewModel = new();
            this.DataContext = viewModel;

            #region ViewModel Events
            viewModel.CompleteSignIn += ViewModel_CompleteSignIn;
            #endregion

            #region ViewModel Bindings
            Binding ipAddressBinding = new(nameof(viewModel.IpAddress));
            ipAddressBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            this.textBoxIpAddress.SetBinding(TextBox.TextProperty, ipAddressBinding);

            Binding portBinding = new(nameof(viewModel.Port));
            portBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            this.textBoxPort.SetBinding(TextBox.TextProperty, portBinding);

            Binding clientsBinding = new(nameof(viewModel.Clients));
            clientsBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            this.listBoxClients.SetBinding(ListBox.ItemsSourceProperty, clientsBinding);

            Binding selectedClientBinding = new(nameof(viewModel.SelectedClient));
            selectedClientBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            this.listBoxClients.SetBinding(ListBox.SelectedItemProperty, selectedClientBinding);
            #endregion

            #region ViewModel Commands
            this.buttonStart.Command = viewModel.StartCommand;
            this.buttonSend.Command = viewModel.SendCommand;
            this.buttonOpenSignInView.Command = viewModel.SignInCommand;
            #endregion
        }

        private void ViewModel_CompleteSignIn()
        {
            new SignInView().Show();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            Environment.Exit(0);
        }
    }
}
