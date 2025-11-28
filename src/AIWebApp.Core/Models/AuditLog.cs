namespace AIWebApp.Core.Models;

public class AuditLog
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Feature { get; set; } = string.Empty; // Chatbot, Sentiment, ImageAnalysis, etc.
    public DateTime Timestamp { get; set; }
    public string? Details { get; set; }
}
