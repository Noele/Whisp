﻿using System.Text.Json;
using NAudio.Wave;
using Vosk;

namespace Whisp.Whisp;

public sealed class Listener
{
    private const string Path = @".\AIModels\vosk-model-en-us-0.22";
    private const int SampleRate = 16000;
    private const string Keyword = "whisper";
    
    private readonly Model _model;
    private VoskRecognizer _recognizer;
    private WaveInEvent _waveIn;
    
    public event WhispMessageEvent OnWhispMessageEvent;
    public delegate void WhispMessageEvent(string message);
    
    public Listener()
    {
        Environment.SetEnvironmentVariable("VOSK_CUDA", "1");
        
        this._model = new Model(Path);
        this._recognizer = new VoskRecognizer(this._model, SampleRate);
        this._recognizer.SetMaxAlternatives(3);
        this._waveIn = new WaveInEvent
        {
            WaveFormat = new WaveFormat(SampleRate, 1),
            BufferMilliseconds = 50,
        };
        
        this._waveIn.DataAvailable += (_, e) =>
        {
            if (!this._recognizer.AcceptWaveform(e.Buffer, e.BytesRecorded)) return;

            var finalResult = this._recognizer.FinalResult();
            Console.WriteLine(finalResult);
            
            if (finalResult.Contains(Keyword))
            {
                VoskResponse? voskResponse = JsonSerializer.Deserialize<VoskResponse>(finalResult);
                if (voskResponse == null) return;
                
                Alternative? matchingAlternative = voskResponse.alternatives
                    .Where(a => a.text.Contains(Keyword, StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(a => a.confidence)
                    .FirstOrDefault();
                
                if (matchingAlternative == null) return;
                
                var text = matchingAlternative.text;
                var index = text.IndexOf(Keyword, StringComparison.Ordinal);
                var result = index >= 0 ? text[(index + Keyword.Length)..].Trim() : string.Empty;
                result = result.Replace(Keyword, string.Empty).Trim();
                if (result.Length > 0)
                    WhispMessage(result);
            }
        };
    }
    
    public void StartRecording() => this._waveIn.StartRecording();
    public void StopRecording() => this._waveIn.StopRecording();

    private void WhispMessage(string message)
    {
        OnWhispMessageEvent.Invoke(message);
    }
}