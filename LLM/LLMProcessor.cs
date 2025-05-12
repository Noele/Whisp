using System.Text.Json;
using Whisp.Actions.MusicPlayer;
using Whisp.ScreenReader;
using Whisp.Whisp;

namespace Whisp;

public class LLMProcessor
{
    private const string ScreenshotPrompt =
        "This is a screenshot run through OCR, give information about what you see, if there are questions, answer them";
    
    private LLMRequestHandler _handler;
    private ScreenshotReader _screenshotReader;
    private Speaker _speaker;
    
    private MusicPlayer _musicPlayer;

    private readonly Dictionary<string, Func<string, Task>> _llmFunctionLookupTable;

    public LLMProcessor(ScreenshotReader screenshotReader, Speaker speaker, Listener listener)
    {
        this._musicPlayer = new MusicPlayer(speaker);
        this._handler = new LLMRequestHandler();
        this._screenshotReader = screenshotReader;
        this._speaker = speaker;
        this._llmFunctionLookupTable = new Dictionary<string, Func<string, Task>>
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
            this._speaker.Speak("Resetting chat history");
            await this._handler.ClearChatHistroyTask();
            return;
        }
        string response = await this._handler.SendRequestAsync(msg);
        Console.WriteLine($"LLM: {response}");

        var match = this._llmFunctionLookupTable.Keys
            .FirstOrDefault(key => response.StartsWith(key, StringComparison.OrdinalIgnoreCase));
        if(match != null)
            await this._llmFunctionLookupTable[match].Invoke(response);
        else
            this._speaker.Speak(response);
    }
    
    private async Task HandleScreenshot(string msg)
    {
        this._speaker.Speak("Processing screenshot");
        string text = this._screenshotReader.ProcessScreenshot();
        text = $"{ScreenshotPrompt}\n{text}";
        string response = await this._handler.SendRequestAsync(text);
        this._speaker.Speak(response);
    }

    private async Task HandlePlay(string msg)
    {
        msg = msg.Replace("[play]", string.Empty);
        var responseJson = JsonSerializer.Deserialize<JsonElement>(msg);
        this._speaker.Speak(responseJson.GetProperty("message").ToString());
        await this._musicPlayer.Play($"{responseJson.GetProperty("name")} by {responseJson.GetProperty("artist")}");
    }

    private async Task HandleStop(string msg)
    {
        msg = msg.Replace("[stop]", string.Empty);
        await this._musicPlayer.Stop();
        var responseJson = JsonSerializer.Deserialize<JsonElement>(msg);
        this._speaker.Speak(responseJson.GetProperty("message").ToString());
    }

    private async Task HandleTimer(string msg)
    {
        msg = msg.Replace("[timer]", string.Empty);
        var responseJson = JsonSerializer.Deserialize<JsonElement>(msg);
        Actions.Timer.Timer.StartTimer(int.Parse(responseJson.GetProperty("length").ToString()), this._speaker);
    }
}