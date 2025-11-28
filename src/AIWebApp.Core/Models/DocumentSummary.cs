namespace AIWebApp.Core.Models;

public class DocumentSummary
{
    public string OriginalText { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public int OriginalLength { get; set; }
    public int SummaryLength { get; set; }
    public List<string> KeyPoints { get; set; } = new();
}
