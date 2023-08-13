using Messenger.BL;
using System;
using Messenger.Models.Application;

namespace Messenger.ViewModels;

public class VoiceRecordControlViewModel : ViewModelBase
{
    public event Action<MultimediaMessage>? CompleteConfirm;
    public event Action? CompleteCancel;

    public CommandBase StartRecordingVoiceCommand { get; }
    public CommandBase StopRecordingVoiceCommand { get; }
    public CommandBase StartListeningVoiceMessageCommand { get; }
    public CommandBase StopListeningVoiceMessageCommand { get; }
    public CommandBase ConfirmCommand { get; }
    public CommandBase CancelCommand { get; }

    private bool isRecording;
    public bool IsRecording
    {
        get => this.isRecording;
        set
        {
            this.isRecording = value;
            this.OnPropertyChanged();
        }
    }
    private byte[]? currentVoiceMessage;
    private readonly VoiceRecorderAdapter voiceRecorder;

    public VoiceRecordControlViewModel()
    {
        #region Initialize Commands
        this.StartRecordingVoiceCommand = new(this.StartRecordingVoice);
        this.StopRecordingVoiceCommand = new(this.StopRecordingVoice);
        this.StartListeningVoiceMessageCommand = new(this.StartListeningVoice);
        this.StopListeningVoiceMessageCommand = new(this.StopListeningVoice);
        this.ConfirmCommand = new(this.Confirm);
        this.CancelCommand = new(this.Cancel);
        #endregion

        this.voiceRecorder = new();
    }

    private void StartRecordingVoice(object? obj)
    {
        if (this.voiceRecorder.CanStartRecording())
        {
            this.IsRecording = true;
            this.voiceRecorder.StartRecording();
        }
    }

    private void StopRecordingVoice(object? obj)
    {
        if (this.voiceRecorder.CanStopRecording())
        {
            this.IsRecording = false;
            this.currentVoiceMessage = this.voiceRecorder.StopRecording();
        }
    }

    private void StartListeningVoice(object? obj)
    {
        if (this.currentVoiceMessage is not null &&
            this.currentVoiceMessage.Length > 0)
            this.voiceRecorder.PlayAudio(this.currentVoiceMessage);
    }

    private void StopListeningVoice(object? obj) =>
        this.voiceRecorder.StopPlayingAudio();

    private void Confirm(object? obj)
    {
        if (this.currentVoiceMessage is null)
            return;
        MultimediaMessage message = new()
        {
            Content = this.currentVoiceMessage,
            Type = MultimediaMessageType.Audio,
        };
        this.CompleteConfirm?.Invoke(message);
    }

    private void Cancel(object? obj)
    {
        this.voiceRecorder.StopPlayingAudio();
        this.CompleteCancel?.Invoke();
    }
}
