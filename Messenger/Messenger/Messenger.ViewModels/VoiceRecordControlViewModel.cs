using Messenger.Common;
using Messenger.BL;
using System;
using Messenger.Models.Application;
using Newtonsoft.Json;
using System.Text;

namespace Messenger.ViewModels;

public class VoiceRecordControlViewModel : ViewModelBase
{
    public event Action<Message>? CompleteConfirm;
    public event Action? CompleteCancel;

    public CommandBase StartRecordingVoiceCommand { get; }
    public CommandBase PauseRecordingVoiceCommand { get; }
    public CommandBase StopRecordingVoiceCommand { get; }
    public CommandBase StartListeningVoiceMessageCommand { get; }
    public CommandBase StopListeningVoiceMessageCommand { get; }
    public CommandBase ConfirmCommand { get; }
    public CommandBase CancelCommand { get; }

    private VoiceRecorderAdapter voiceRecorder;
    private byte[]? currentVoiceMessage;

    public VoiceRecordControlViewModel()
    {
        #region Initialize Commands
        this.StartRecordingVoiceCommand = new(this.StartRecordingVoiceMessage);
        this.PauseRecordingVoiceCommand = new(this.PauseRecordingVoiceMessage);
        this.StopRecordingVoiceCommand = new(this.StopRecordingVoiceMessage);
        this.StartListeningVoiceMessageCommand = new(this.StartListeningVoiceMessage);
        this.StopListeningVoiceMessageCommand = new(this.StopListeningVoiceMessage);
        this.ConfirmCommand = new(this.Confirm);
        this.CancelCommand = new(this.Cancel);
        #endregion

        this.voiceRecorder = new();
    }

    public void StartRecordingVoiceMessage(object? obj)
    {
        if (this.voiceRecorder.CanStartRecording())
            this.voiceRecorder.StartRecording(VoiceRecorderExtensions.GenerateOutputPath());
    }

    public void PauseRecordingVoiceMessage(object? obj)
    {

    }

    public void StopRecordingVoiceMessage(object? obj)
    {
        if (this.voiceRecorder.CanStopRecording())
            this.currentVoiceMessage = this.voiceRecorder.StopRecording();
    }

    public void StartListeningVoiceMessage(object? obj)
    {
        if (this.currentVoiceMessage is not null &&
            this.currentVoiceMessage.Length > 0)
            this.voiceRecorder.PlayAudio(this.currentVoiceMessage);
    }

    public void StopListeningVoiceMessage(object? obj)
    {
        this.voiceRecorder.StopPlayingAudio();
    }

    public void Confirm(object? obj)
    {
        if (this.currentVoiceMessage is null)
            return;
        Message message = new()
        {
            Content = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this.currentVoiceMessage)),
            Type = MessageType.File,
        };
        this.CompleteConfirm?.Invoke(message);
    }

    public void Cancel(object? obj)
    {
        this.voiceRecorder.StopPlayingAudio();
        this.CompleteCancel?.Invoke();
    }
}
