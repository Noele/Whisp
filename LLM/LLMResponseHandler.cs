using System.Text.Json;
using System.Text.RegularExpressions;
using Whisp.Actions.MusicPlayer;
using Whisp.Actions.ProgramRunner;
using Whisp.ScreenReader;
using Whisp.Whisp;

namespace Whisp;

public class LLMResponseHandler
{
    private const string ScreenshotPrompt =
        "Im going to read out to you all of the text on my screen, please explain what you see";
    
    private LLMProcessor _handler;
    private ScreenshotReader _screenshotReader;
    private Speaker _speaker;
    private ProgramRunner _programRunner;
    private MusicPlayer _musicPlayer;

    private readonly Dictionary<string, Func<JsonElement, Task>> _llmFunctionLookupTable;

    public LLMResponseHandler(Speaker speaker, Listener listener)
    {
        this._musicPlayer = new MusicPlayer(speaker);
        this._handler = new LLMProcessor();
        this._programRunner = new ProgramRunner();
        this._screenshotReader = new ScreenshotReader();
        this._speaker = speaker;
        this._llmFunctionLookupTable = new Dictionary<string, Func<JsonElement, Task>>
        {
            {"[screenshot]", this.HandleScreenshot},
            {"[play]", this.HandlePlay},
            {"[stop]", this.HandleStop},
            {"[timer]", this.HandleTimer},
            {"[program]", this.HandleProgram},
            {"[volume]", this.HandleVolume}
        };
        listener.OnWhispMessageEvent += message =>
        {
            this.Processs(message);
        };
    }

    public async Task Processs(string msg)
    {
        string response = await this._handler.SendRequestAsync(msg);
        Console.WriteLine($"LLM: {response}");

        var match = this._llmFunctionLookupTable.Keys
            .FirstOrDefault(key => response.StartsWith(key, StringComparison.OrdinalIgnoreCase));
        if (match != null)
        {
            response = response.Replace(match, string.Empty).Trim();
            var responseJson = JsonSerializer.Deserialize<JsonElement>(response);
            this._speaker.Speak(responseJson.GetProperty("message").ToString());
            await this._llmFunctionLookupTable[match].Invoke(responseJson);
        }
        else
            this._speaker.Speak(response);
    }
    
    private async Task HandleScreenshot(JsonElement responseJson)
    {
        string response = await this._screenshotReader.ProcessScreenshot();
        Console.WriteLine($"LLM: {response}");
        await this._handler.ReplaceLastAssistantReply(response);
        this._speaker.Speak(response);
    }

    private async Task HandlePlay(JsonElement responseJson)
    {
        await this._musicPlayer.Play($"{responseJson.GetProperty("name")} by {responseJson.GetProperty("artist")}");
    }

    private async Task HandleStop(JsonElement responseJson)
    {
        await this._musicPlayer.Stop();
    }

    private async Task HandleTimer(JsonElement responseJson)
    {
        Actions.Timer.Timer.StartTimer(int.Parse(responseJson.GetProperty("seconds").ToString()), this._speaker);
    }

    private async Task HandleProgram(JsonElement responseJson)
    {
        this._programRunner.Run(responseJson.GetProperty("name").ToString());
    }
    private async Task HandleVolume(JsonElement responseJson)
    {
        SystemController.SetVolume(int.Parse(responseJson.GetProperty("level").ToString()));
    }
}