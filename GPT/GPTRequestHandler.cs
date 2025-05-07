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
        prompt.AppendLine("You are a friend, a companion, and a helper, layed back and chill but you dont mention it");
        prompt.AppendLine("You try to give medium length responses, providing enough information without being too verbose");
        prompt.AppendLine("Your name is Whisp");
        prompt.AppendLine("You do not use emojis");
        prompt.AppendLine("If asked about a screen, respond with [screenshot] and nothing else");
        prompt.AppendLine("If told to look at something, respond with [screenshot] and nothing else");
        var currentDateTime = DateTime.Now;
        var formattedDateTime = currentDateTime.ToString("MM/dd/yyyy 'at' HH:mm");

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
}