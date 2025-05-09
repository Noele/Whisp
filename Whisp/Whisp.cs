using Whisp.ScreenReader;

namespace Whisp.Whisp;

public class Whisp
{
    private Listener listener;

    private Speaker speaker;
    
    private GPTProcessor gptProcessor;
    
    private ScreenshotReader screenshotReader;
    
    public Whisp()
    {
        this.listener = new Listener();
        this.speaker = new Speaker();
        this.screenshotReader = new ScreenshotReader();
        this.gptProcessor = new GPTProcessor(this.screenshotReader, this.speaker, this.listener);
    }

    public async Task Run(string[] args)
    {
        listener.StartRecording();
        Console.WriteLine("Listening...");
        await Task.Delay(-1);
    }
}