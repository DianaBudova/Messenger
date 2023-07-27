using Messenger.ViewModels;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Configuration;
using Messenger.Repositories;
using System.Collections.Generic;
using Messenger.Models.DB;
using System.Linq;
using System.Net.Sockets;
using System.Net;

namespace Messenger.Views
{
    /// <summary>
    /// Interaction logic for ServerView.xaml
    /// </summary>
    public partial class ServerView : Window
    {
        private readonly ServerViewModel viewModel;

        public ServerView()
        {
            InitializeComponent();

            viewModel = new();
            this.DataContext = viewModel;

            #region ViewModel Events
            viewModel.CompleteSignIn += ViewModel_CompleteSignIn;
            viewModel.CompleteExit += ViewModel_CompleteExit;
            #endregion

            #region ViewModel Bindings
            Binding clientsBinding = new(nameof(viewModel.Clients));
            clientsBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            this.listBoxClients.SetBinding(ListBox.ItemsSourceProperty, clientsBinding);

            Binding messagesBinding = new(nameof(viewModel.Messages));
            messagesBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            this.listBoxMessages.SetBinding(ListBox.ItemsSourceProperty, messagesBinding);

            Binding selectedClientBinding = new(nameof(viewModel.SelectedClient));
            selectedClientBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            this.listBoxClients.SetBinding(ListBox.SelectedItemProperty, selectedClientBinding);

            Binding isStartedBinding = new(nameof(viewModel.IsStarted));
            isStartedBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            Style style = new();
            DataTrigger isStartedTrue = new()
            {
                Binding = isStartedBinding,
                Value = true,
            };
            isStartedTrue.Setters.Add(new Setter()
            {
                Property = Image.SourceProperty,
                Value = new BitmapImage(new Uri($"{ConfigurationManager.AppSettings["ImagesPath"]}GreenCircle.png", UriKind.Relative)),
            });
            DataTrigger isStartedFalse = new()
            {
                Binding = isStartedBinding,
                Value = false,
            };
            isStartedFalse.Setters.Add(new Setter()
            {
                Property = Image.SourceProperty,
                Value = new BitmapImage(new Uri($"{ConfigurationManager.AppSettings["ImagesPath"]}RedCircle.png", UriKind.Relative)),
            });
            style.Triggers.Add(isStartedTrue);
            style.Triggers.Add(isStartedFalse);
            this.imageStatus.Style = style;
            #endregion

            #region ViewModel Commands
            this.buttonStart.Command = viewModel.StartCommand;
            this.buttonStop.Command = viewModel.StopCommand;
            this.buttonOpenSignInView.Command = viewModel.SignInCommand;
            #endregion
        }

        private void ViewModel_CompleteSignIn()
        {
            List<Server>? servers = RepositoryFactory.GetServerRepository().GetAll();
            if (servers is not null ||
                servers!.Select(s => s.NameServer).Contains(ConfigurationManager.AppSettings["ServerNameByDefault"]))
                new SignInView(servers!).Show();
        }

        private void ViewModel_CompleteExit()
        {
            Dispatcher.Invoke(this.Close);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.viewModel.StopCommand.Execute(null);
            base.OnClosing(e);
        }
    }
}
