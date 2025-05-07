using System.Speech.Synthesis;

namespace Whisp;

public class Speaker
{
    private SpeechSynthesizer synthesizer;
    public Speaker()
    {
        this.synthesizer = new System.Speech.Synthesis.SpeechSynthesizer();
        this.synthesizer.SetOutputToDefaultAudioDevice();
        
        this.synthesizer.SelectVoice("Microsoft Zira Desktop");
    }
    
    public void Speak(string text)
    {
        this.synthesizer.Speak(text);
    }
}