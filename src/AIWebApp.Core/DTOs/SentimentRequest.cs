namespace AIWebApp.Core.DTOs;

public class SentimentRequest
{
    public string Text { get; set; } = string.Empty;
}

public class SentimentResponse
{
    public string Sentiment { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public string Explanation { get; set; } = string.Empty;
}
