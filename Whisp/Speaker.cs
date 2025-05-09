using NetMQ;
using NetMQ.Sockets;

namespace Whisp;

public class Speaker
{
    private HttpClient client = new HttpClient();
    private const int PORT = 5555;
    private const string HOST = "tcp://localhost";
    private string url = $"{HOST}:{PORT}";
    private PushSocket _requestSocket;
    public Speaker()
    {
        this._requestSocket = new PushSocket();
    }
    
    public async Task Speak(string text)
    {
        this._requestSocket.Connect(url);
        this._requestSocket.SendFrame(text);
        Console.WriteLine($"Sending payload: \n{text}");
    }
}