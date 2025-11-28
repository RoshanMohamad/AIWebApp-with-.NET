namespace AIWebApp.Core.Models;

public class SentimentResult
{
    public string Text { get; set; } = string.Empty;
    public string Sentiment { get; set; } = string.Empty; // Positive, Negative, Neutral
    public double ConfidenceScore { get; set; }
    public string? Explanation { get; set; }
}
