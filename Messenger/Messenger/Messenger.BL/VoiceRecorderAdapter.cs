using Messenger.Models.Application;

namespace Messenger.BL;

public class VoiceRecorderAdapter
{
    public VoiceRecorderState State { get; private set; }
    private readonly VoiceRecorder recorder;

    public VoiceRecorderAdapter()
    {
        this.recorder = new();
        this.State = VoiceRecorderState.None;
    }

    public void StartRecording(string? outputPath)
    {
        if (outputPath is null)
        {
            this.State = VoiceRecorderState.None;
            return;
        }
        this.State = VoiceRecorderState.Records;
        this.recorder.StartRecording(outputPath);
    }

    public byte[] StopRecording()
    {
        this.State = VoiceRecorderState.NonRecords;
        return this.recorder.StopRecording();
    }

    public void PlayAudio(byte[] audio)
    {
        this.recorder.PlayAudio(audio);
    }

    public void StopPlayingAudio()
    {
        this.recorder.StopPlayingAudio();
    }

    public bool CanStartRecording()
    {
        if (this.State == VoiceRecorderState.None ||
            this.State == VoiceRecorderState.NonRecords)
            return true;
        return false;
    }

    public bool CanStopRecording()
    {
        if (this.State == VoiceRecorderState.Records)
            return true;
        return false;
    }
}
