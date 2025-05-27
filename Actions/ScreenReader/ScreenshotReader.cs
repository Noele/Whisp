using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Text.RegularExpressions;
using LLama;
using LLama.Common;
using LLama.Sampling;
#pragma warning disable CA1416

namespace Whisp.ScreenReader;

public class ScreenshotReader
{
    private const string MultiModalProj = @".\AIModels\llava-llama-3-8b-v1_1-mmproj-f16.gguf";
    private const string ModelPath = @".\AIModels\llava-llama-3-8b-v1_1-int4.gguf";
    private const string ModelImage = @".\Screen.png";
    private const int MaxTokens = 2048;
    private string _prompt;
    private InteractiveExecutor interactiveExecutor;
    private readonly InferenceParams inferenceParams;
    public ScreenshotReader()
    {
        this._prompt = "{<image>}\nUser: What is shown in this image?\nAssistant:";

        
        var parameters = new ModelParams(ModelPath)
        {
            ContextSize = 4096,
            GpuLayerCount = 40,
            MainGpu = 0,
        };

        var model = LLamaWeights.LoadFromFile(parameters);
        var context = model.CreateContext(parameters);
        // Llava Init
        var clipModel = LLavaWeights.LoadFromFile(MultiModalProj);

        this.interactiveExecutor = new InteractiveExecutor(context, clipModel );

        this.inferenceParams = new InferenceParams() { 
            SamplingPipeline = new DefaultSamplingPipeline
            {
                Temperature = 0.5f
            },
            AntiPrompts = new List<string> { "\nUSER:" }, 
            MaxTokens = MaxTokens
            
        };
    }

    public async Task<string> ProcessScreenshot()
    {
        using Bitmap bitmap = new Bitmap(2560, 1440);
        using Graphics g = Graphics.FromImage(bitmap);
            
        // Capture the screenshot.
        g.CopyFromScreen(0, 0, 0, 0, new Size(2560, 1440));
        using var resized = new Bitmap(bitmap, new Size(224, 224));
        resized.Save(ModelImage, ImageFormat.Png);
        
        this.interactiveExecutor.Images.Clear();
        var bytes = await File.ReadAllBytesAsync(ModelImage);
        this.interactiveExecutor.Images.Add(bytes);

        StringBuilder output = new StringBuilder();
        await foreach (var text in this.interactiveExecutor.InferAsync(this._prompt, this.inferenceParams))
        {
            output.Append(text);
        }
        return output.ToString();
    }
}