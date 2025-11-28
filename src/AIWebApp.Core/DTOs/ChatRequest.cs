namespace AIWebApp.Core.DTOs;

public class ChatRequest
{
    public string Message { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string? SessionId { get; set; }
}

public class ChatResponse
{
    public string Response { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
