namespace Whisp.Whisp;

public class Alternative
{
    public double confidence { get; set; }
    public string text { get; set; }
}

public class VoskResponse
{
    public List<Alternative> alternatives { get; set; }
}

