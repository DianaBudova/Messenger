using NAudio.Wave;

namespace Messenger.BL;

internal class VoiceRecorder
{
    private WaveInEvent? waveIn;
    private WaveOutEvent? waveOut;
    private WaveFileWriter? waveWriter;
    private WaveFileReader? waveReader;

    public void StartRecording(string? outputPath)
    {
        if (outputPath is null)
            return;
        this.waveIn = new();
        this.waveIn.DataAvailable += WaveIn_DataAvailable;
        this.waveIn.WaveFormat = new(44100, 1);
        this.waveWriter = new(outputPath, this.waveIn.WaveFormat);
        if (this.waveWriter.CanWrite)
            this.waveIn.StartRecording();
    }

    public byte[] StopRecording()
    {
        if (this.waveIn is null)
            return new byte[0];
        this.waveIn.StopRecording();
        this.waveIn.Dispose();

        if (this.waveWriter is null)
            return new byte[0];
        this.waveWriter.Close();
        this.waveWriter.Dispose();

        return File.ReadAllBytes(this.waveWriter!.Filename);
    }

    public void PlayAudio(byte[]? audio)
    {
        if (audio is null)
            return;
        this.waveReader = new(new MemoryStream(audio));
        this.waveOut = new();
        this.waveOut.Init(this.waveReader);
        this.waveOut.Play();
    }

    public void StopPlayingAudio()
    {
        this.waveOut?.Stop();
    }

    private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
    {
        if (this.waveWriter is null)
            return;
        this.waveWriter.Write(e.Buffer, 0, e.BytesRecorded);
        this.waveWriter.Flush();
    }
}
