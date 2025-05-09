namespace Whisp.Actions.Timer;

public class Timer
{
    public static async Task StartTimer(int seconds, Speaker speaker)
    {
        await speaker.Speak($"Setting timer for {seconds} seconds");
        await Task.Delay(seconds * 1000);
        await speaker.Speak($"Your timer for {seconds} seconds has finished");
    }
}