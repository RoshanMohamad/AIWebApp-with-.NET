namespace AIWebApp.Core.Models;

public class ImageAnalysisResult
{
    public string Description { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public List<DetectedObject> Objects { get; set; } = new();
    public string? Text { get; set; }
}

public class DetectedObject
{
    public string Name { get; set; } = string.Empty;
    public double Confidence { get; set; }
}
