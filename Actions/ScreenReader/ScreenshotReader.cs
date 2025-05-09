using System.Drawing;
using Tesseract;

namespace Whisp.ScreenReader;

public class ScreenshotReader
{
    private static readonly Rectangle bounds = new Rectangle(0, 0, 2560, 1440);
    private TesseractEngine tesseractEngine;
    public ScreenshotReader()
    {
        this.tesseractEngine = new TesseractEngine(@"C:\Users\zoeyn\RiderProjects\Whisp\Whisp\", "eng", EngineMode.Default);
    }

    public string ProcessScreenshot()
    { 
        using var bitmap = new Bitmap(bounds.Width, bounds.Height);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bitmap.Size);
        
        using var pix = PixConverter.ToPix(bitmap);
        using var page = this.tesseractEngine.Process(bitmap);
        
        var text = page.GetText();

        return text;
    }
}