using Messenger.ViewModels;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Configuration;

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

            this.viewModel = new();
            this.DataContext = viewModel;

            #region ViewModel Events
            viewModel.CompleteExit += this.ViewModel_CompleteExit;
            #endregion

            #region ViewModel Bindings
            Binding clientsBinding = new(nameof(viewModel.Clients));
            clientsBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            this.listBoxClients.SetBinding(ListBox.ItemsSourceProperty, clientsBinding);

            Binding messagesBinding = new(nameof(viewModel.Messages));
            messagesBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            this.listBoxMessages.SetBinding(ListBox.ItemsSourceProperty, messagesBinding);

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
            #endregion
        }

        private void ViewModel_CompleteExit() =>
            Dispatcher.Invoke(this.Close);

        protected override void OnClosing(CancelEventArgs e)
        {
            this.viewModel.StopCommand.Execute(null);
            base.OnClosing(e);
        }
    }
}
