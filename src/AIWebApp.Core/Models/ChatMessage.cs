namespace AIWebApp.Core.Models;

public class ChatMessage
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Response { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? SessionId { get; set; }
}
