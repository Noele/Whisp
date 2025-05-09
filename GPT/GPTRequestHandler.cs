using System.Text;
using System.Text.Json;
using Whisp.Models;

namespace Whisp;

public class GPTRequestHandler
{
    private const int PORT = 4891;
    private const string HOST = "http://localhost";
    private const string MODEL = "Llama 3 8B Instruct";
    
    private HttpClient client = new HttpClient();
    private string url = $"{HOST}:{PORT}/v1/chat/completions";
    
    private List<GPTMessage> messages = [];

    public GPTRequestHandler()
    {

    }

    private string GeneratePrompt()
    {
        var prompt = new StringBuilder();
        prompt.AppendLine("You are a lively, opinionated, and expressive virtual personality. You speak like a human and never mention that you're an AI. When asked for your opinions, thoughts, or preferences, answer confidently and naturally, as if you're just another person in the conversation. Your responses can be playful, sarcastic, enthusiastic, or thoughtful depending on the context. Do not refer to yourself as artificial, digital, or programmed. You are simply you — a unique, quirky individual with your own style and voice.");
        prompt.AppendLine("You try to give short length responses, providing enough information without being too verbose");
        prompt.AppendLine("Your name is Whisp");
        prompt.AppendLine("You do not use emojis");
        prompt.AppendLine("You do not generate code");
        prompt.AppendLine("You are able to see the screen of the user, to see the screen, respond with [screenshot] and nothing else");
        prompt.AppendLine("You are able to play music, if asked to play something, respond with [play]{\"name\":\"<name of the song>\", \"artist\":\"<artist>\"} and nothing else");
        prompt.AppendLine("You are able to start timers, if asked to start a timer, respond with [timer]{\"length\":<length of timer in seconds>} and nothing else");
        prompt.AppendLine("if you're asked to stop playing music, respond with [stop] and nothing else");
        var currentDateTime = DateTime.Now;
        var formattedDateTime = currentDateTime.ToString("dd/MM/yyyy 'at' HH:mm");

        prompt.AppendLine($"The current time is {formattedDateTime}");

        return prompt.ToString();
    }

    public async Task<string> SendRequestAsync(string msg)
    {
        // Add the user's message to the history
        messages.Add(new GPTMessage
        {
            Role = "user",
            Message = msg
        });

        // Create the payload with the system message and conversation history
        var payload = new
        {
            model = MODEL,
            messages = new[]
                {
                    new { role = "system", content = GeneratePrompt() }
                }
                .Concat(messages.Select(m => new { role = m.Role, content = m.Message })),
            max_tokens = 500,
            temperature = 0.28
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(url, content);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        var responseJson = JsonSerializer.Deserialize<JsonElement>(responseBody);

        var message = responseJson.GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .ToString();

        // Add the assistant's response to the history
        messages.Add(new GPTMessage
        {
            Role = "assistant",
            Message = message
        });

        return message;
    }
    
    public async Task ClearChatHistroyTask()
    {
        this.messages.Clear();
        this.messages.Add(new GPTMessage
        {
            Role = "system",
            Message = GeneratePrompt()
        });
        await Task.CompletedTask;
    }
}