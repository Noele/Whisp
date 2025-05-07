using System.Text.Json;
using NAudio.Wave;
using Vosk;

namespace Whisp;

public class Listener
{
    private const string PATH = "C:\\Users\\zoeyn\\RiderProjects\\Whisp\\Whisp\\vosk-model-en-us-0.42-gigaspeech";
    private const int SAMPLE_RATE = 16000;
    private const string KEYWORD = "whisper";
    
    private Vosk.Model model;
    private VoskRecognizer recognizer;
    private WaveInEvent waveIn;
    
    public event WhispMessageEvent OnWhispMessageEvent;
    public delegate void WhispMessageEvent(string message);
    
    public Listener()
    {
        Environment.SetEnvironmentVariable("VOSK_CUDA", "1");
        
        this.model = new Model(PATH);
        this.recognizer = new VoskRecognizer(this.model, SAMPLE_RATE);
        this.waveIn = new WaveInEvent
        {
            WaveFormat = new WaveFormat(SAMPLE_RATE, 1),
            BufferMilliseconds = 50,
        };
        
        this.waveIn.DataAvailable += (sender, e) =>
        {
            if (!this.recognizer.AcceptWaveform(e.Buffer, e.BytesRecorded)) return;
            
            var finalResult = this.recognizer.FinalResult().ToLower();
            Console.WriteLine(finalResult);
            
            if (finalResult.Contains(KEYWORD))
            {
                var text = JsonDocument.Parse(finalResult).RootElement.GetProperty("text").ToString();
                var index = text.IndexOf(KEYWORD, StringComparison.Ordinal);
                var result = index >= 0 ? text[(index + KEYWORD.Length)..].Trim() : string.Empty;
                result = result.Replace(KEYWORD, string.Empty).Trim();
                if (result.Length > 0)
                    WhispMessage(result);
            }
        };
    }
    
    public void StartRecording() => this.waveIn.StartRecording();
    public void StopRecording() => this.waveIn.StopRecording();

    protected virtual void WhispMessage(string message)
    {
        OnWhispMessageEvent?.Invoke(message);
    }
}