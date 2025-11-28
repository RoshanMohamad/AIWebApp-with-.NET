namespace AIWebApp.Core.Models;

public class User
{
    public int Id { get; set; }
    public string UserId { get; set; } = Guid.NewGuid().ToString();
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime LastLoginAt { get; set; }
}
