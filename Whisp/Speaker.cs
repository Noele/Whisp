using NetMQ;
using NetMQ.Sockets;

namespace Whisp;

public class Speaker
{
    private const int Port = 5555;
    private const string Host = "tcp://localhost";
    private readonly string _url = $"{Host}:{Port}";
    private PushSocket _requestSocket;
    public Speaker()
    {
        this._requestSocket = new PushSocket();
    }
    
    public async Task Speak(string text)
    {
        this._requestSocket.Connect(_url);
        this._requestSocket.SendFrame(text);
        Console.WriteLine($"Sending payload: \n{text}");

    }
}
