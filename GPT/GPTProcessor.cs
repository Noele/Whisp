using System.Text.Json;
using Whisp.Actions.MusicPlayer;
using Whisp.ScreenReader;

namespace Whisp;

public class GPTProcessor
{
    private const string SCREENSHOT_PROMPT =
        "This is a screenshot run through OCR, give information about what you see, if there are questions, answer them";
    
    private GPTRequestHandler handler;
    private ScreenshotReader screenshotReader;
    private Speaker speaker;
    
    private MusicPlayer musicPlayer;

    private Dictionary<string, Func<string, Task>> GPTFunctionLookupTable;

    public GPTProcessor(ScreenshotReader screenshotReader, Speaker speaker, Listener listener)
    {
        this.musicPlayer = new MusicPlayer(speaker);
        this.handler = new GPTRequestHandler();
        this.screenshotReader = screenshotReader;
        this.speaker = speaker;
        this.GPTFunctionLookupTable = new Dictionary<string, Func<string, Task>>
        {
            {"[screenshot]", this.HandleScreenshot},
            {"[play]", this.HandlePlay},
            {"[stop]", this.HandleStop},
            {"[timer]", this.HandleTimer}
        };
        listener.OnWhispMessageEvent += message =>
        {
            this.Processs(message);
        };
    }

    public async Task Processs(string msg)
    {
        if (msg == "clear")
        {
            this.speaker.Speak("Resetting chat history");
            await this.handler.ClearChatHistroyTask();
            return;
        }
        string response = await this.handler.SendRequestAsync(msg);
        Console.WriteLine($"GPT: {response}");

        var match = this.GPTFunctionLookupTable.Keys
            .FirstOrDefault(key => response.StartsWith(key, StringComparison.OrdinalIgnoreCase));
        if(match != null)
            await this.GPTFunctionLookupTable[match].Invoke(response);
        else
            this.speaker.Speak(response);
    }
    
    private async Task HandleScreenshot(string msg)
    {
        this.speaker.Speak("Processing screenshot");
        string text = this.screenshotReader.ProcessScreenshot();
        text = $"{SCREENSHOT_PROMPT}\n{text}";
        string response = await this.handler.SendRequestAsync(text);
        this.speaker.Speak(response);
    }

    private async Task HandlePlay(string msg)
    {
        msg = msg.Replace("[play]", string.Empty);
        var responseJson = JsonSerializer.Deserialize<JsonElement>(msg);
        await this.musicPlayer.Play($"{responseJson.GetProperty("name")} by {responseJson.GetProperty("artist")}");
    }

    private async Task HandleStop(string msg)
    {
        await this.musicPlayer.Stop();
    }

    private async Task HandleTimer(string msg)
    {
        msg = msg.Replace("[timer]", string.Empty);
        var responseJson = JsonSerializer.Deserialize<JsonElement>(msg);
        Actions.Timer.Timer.StartTimer(int.Parse(responseJson.GetProperty("length").ToString()), this.speaker);
    }
}