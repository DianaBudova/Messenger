using NAudio.Wave;

namespace Messenger.BL;

internal class VoiceRecorder
{
    private WaveInEvent? waveIn;
    private WaveOutEvent? waveOut;
    private MemoryStream? audioMemoryStream;
    private BufferedWaveProvider? bufferedWaveProvider;

    public void StartRecording()
    {
        this.audioMemoryStream = new MemoryStream();
        this.bufferedWaveProvider = new BufferedWaveProvider(new WaveFormat(44100, 1));

        this.waveIn = new WaveInEvent();
        this.waveIn.DataAvailable += WaveIn_DataAvailable;
        this.waveIn.WaveFormat = new WaveFormat(44100, 1);
        this.waveIn.StartRecording();
    }

    public byte[] StopRecording()
    {
        this.waveIn?.StopRecording();
        this.waveIn?.Dispose();

        byte[] audioData = this.audioMemoryStream?.ToArray() ?? new byte[0];

        this.audioMemoryStream?.Dispose();
        this.bufferedWaveProvider = null;

        return audioData;
    }

    public void PlayAudio(byte[]? audio)
    {
        if (audio is null)
            return;

        this.audioMemoryStream = new MemoryStream(audio);
        this.bufferedWaveProvider = new BufferedWaveProvider(new WaveFormat(44100, 1));
        this.bufferedWaveProvider.AddSamples(audio, 0, audio.Length);

        this.waveOut = new WaveOutEvent();
        this.waveOut.Init(this.bufferedWaveProvider);
        this.waveOut.Play();
    }

    public void StopPlayingAudio()
    {
        this.waveOut?.Stop();
    }

    private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
    {
        this.audioMemoryStream?.Write(e.Buffer, 0, e.BytesRecorded);
    }
}