using System.Drawing;
using Whisp.ScreenReader;

namespace Whisp.Whisp;

public class Whisp
{
    private Listener _listener;
    private Speaker _speaker;
    private LLMResponseHandler _llmResponseHandler;
    
    public Whisp()
    {
        this._listener = new Listener();
        this._speaker = new Speaker();
        this._llmResponseHandler = new LLMResponseHandler(this._speaker, this._listener);
    }

    public async Task Run(string[] args)
    {
        _listener.StartRecording();
        Console.WriteLine("Listening...");
        await Task.Delay(-1);
    }
}