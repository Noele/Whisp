using Whisp.ScreenReader;

namespace Whisp;

public class GPTProcessor
{
    private const string SCREENSHOT_PROMPT =
        "This is a screenshot run through OCR, give information about what you see, if there are questions, answer them";
    
    private GPTRequestHandler handler;
    private ScreenshotReader screenshotReader;
    private Speaker speaker;
    

    private Dictionary<string, Func<Task>> GPTFunctionLookupTable;

    public GPTProcessor(ScreenshotReader screenshotReader, Speaker speaker)
    {
        this.handler = new GPTRequestHandler();
        this.screenshotReader = screenshotReader;
        this.speaker = speaker;
        this.GPTFunctionLookupTable = new Dictionary<string, Func<Task>>
        {
            {"[screenshot]", this.HandleScreenshot}
        };
    }

    public async Task Processs(string msg)
    {
        string response = await this.handler.SendRequestAsync(msg);
        if(this.GPTFunctionLookupTable.ContainsKey(response))
            await this.GPTFunctionLookupTable[response].Invoke();
        else
            this.speaker.Speak(response);
        Console.WriteLine($"GPT: {response}");
    }
    
    private async Task HandleScreenshot()
    {
        this.speaker.Speak("Processing screenshot");
        string text = this.screenshotReader.ProcessScreenshot();
        text = $"{SCREENSHOT_PROMPT}\n{text}";
        string response = await this.handler.SendRequestAsync(text);
        this.speaker.Speak(response);
    }
}