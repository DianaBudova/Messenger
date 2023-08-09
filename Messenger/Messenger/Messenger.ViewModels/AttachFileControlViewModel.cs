using Messenger.Models.Application;
using Newtonsoft.Json;
using System;
using System.Text;

namespace Messenger.ViewModels;

public class AttachFileControlViewModel : ViewModelBase
{
    public event Func<File?>? CompleteAttachment;
    public event Action<MultimediaMessage>? CompleteConfirm;
    public event Action? CompleteCancel;

    public CommandBase AttachFileCommand { get; }
    public CommandBase ConfirmCommand { get; }
    public CommandBase CancelCommand { get; }

    private string? path;
    public string? Path
    {
        get => this.path;
        set
        {
            this.path = value;
            this.OnPropertyChanged();
        }
    }
    private long size;
    public long Size
    {
        get => this.size;
        set
        {
            this.size = value;
            this.OnPropertyChanged();
        }
    }
    private File? currentFile;

    public AttachFileControlViewModel()
    {
        #region Initialize Commands
        this.AttachFileCommand = new(this.AttachFile);
        this.ConfirmCommand = new(this.Confirm);
        this.CancelCommand = new(this.Cancel);
        #endregion
    }

    private void AttachFile(object obj)
    {
        this.currentFile = this.CompleteAttachment?.Invoke();
        this.Path = this.currentFile?.Path ?? string.Empty;
        this.Size = this.currentFile?.SizeBytes ?? 0;
    }

    private void Confirm(object obj)
    {
        if (this.currentFile is null)
            return;
        MultimediaMessage message = new()
        {
            Content = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this.currentFile)),
            Type = MultimediaMessageType.File,
        };
        this.CompleteConfirm?.Invoke(message);
    }

    private void Cancel(object obj) =>
        this.CompleteCancel?.Invoke();
}
