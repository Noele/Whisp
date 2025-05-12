using Whisp.ScreenReader;

namespace Whisp.Whisp;

public class Whisp
{
    private Listener _listener;
    private Speaker _speaker;
    private LLMProcessor _llmProcessor;
    private ScreenshotReader _screenshotReader;
    
    public Whisp()
    {
        this._listener = new Listener();
        this._speaker = new Speaker();
        this._screenshotReader = new ScreenshotReader();
        this._llmProcessor = new LLMProcessor(this._screenshotReader, this._speaker, this._listener);
    }

    public async Task Run(string[] args)
    {
        _listener.StartRecording();
        Console.WriteLine("Listening...");
        await Task.Delay(-1);
    }
}