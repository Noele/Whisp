
using AudioSwitcher.AudioApi.CoreAudio;

public class SystemController
{
    public static void SetVolume(int volume)
    {
            var defaultDevice = new CoreAudioController().DefaultPlaybackDevice;
            defaultDevice.SetVolumeAsync(volume);
            Console.WriteLine($"Volume set to {volume}%.");
    }
}