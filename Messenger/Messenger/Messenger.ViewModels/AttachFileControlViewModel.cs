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

    public CommandBase AttachFileCommand { get; set; }
    public CommandBase ConfirmCommand { get; set; }
    public CommandBase CancelCommand { get; set; }

    private string? path;
    private long size;
    public string Path
    {
        get
        { return this.path; }
        set
        {
            this.path = value;
            this.OnPropertyChanged();
        }
    }
    public long Size
    {
        get
        { return this.size; }
        set
        {
            this.size = value;
            this.OnPropertyChanged();
        }
    }

    private File? currentFile { get; set; }

    public AttachFileControlViewModel()
    {
        this.AttachFileCommand = new(this.AttachFile);
        this.ConfirmCommand = new(this.Confirm);
        this.CancelCommand = new(this.Cancel);
    }

    public void AttachFile(object obj)
    {
        this.currentFile = this.CompleteAttachment?.Invoke();
        this.Path = currentFile?.Path ?? string.Empty;
        this.Size = currentFile?.SizeBytes ?? 0;
    }

    public void Confirm(object obj)
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

    public void Cancel(object obj)
    {
        this.CompleteCancel?.Invoke();
    }
}
