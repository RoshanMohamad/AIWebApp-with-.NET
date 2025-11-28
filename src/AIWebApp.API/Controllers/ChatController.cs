using AIWebApp.Core.DTOs;
using AIWebApp.Core.Interfaces;
using AIWebApp.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace AIWebApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IAIService _aiService;
    private readonly IRepository<ChatMessage> _chatRepository;
    private readonly IRepository<AuditLog> _auditRepository;

    public ChatController(IAIService aiService, IRepository<ChatMessage> chatRepository, IRepository<AuditLog> auditRepository)
    {
        _aiService = aiService;
        _chatRepository = chatRepository;
        _auditRepository = auditRepository;
    }

    [HttpPost]
    public async Task<ActionResult<ChatResponse>> SendMessage([FromBody] ChatRequest request)
    {
        try
        {
            var userId = request.UserId ?? "anonymous";
            var sessionId = request.SessionId ?? Guid.NewGuid().ToString();

            var response = await _aiService.GetChatResponseAsync(request.Message, sessionId);

            // Save to database
            var chatMessage = new ChatMessage
            {
                UserId = userId,
                Message = request.Message,
                Response = response,
                Timestamp = DateTime.UtcNow,
                SessionId = sessionId
            };
            await _chatRepository.AddAsync(chatMessage);

            // Log action
            await _auditRepository.AddAsync(new AuditLog
            {
                UserId = userId,
                Action = "SendMessage",
                Feature = "Chatbot",
                Timestamp = DateTime.UtcNow,
                Details = $"Session: {sessionId}"
            });

            return Ok(new ChatResponse
            {
                Response = response,
                SessionId = sessionId,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("history/{userId}")]
    public async Task<ActionResult<IEnumerable<ChatMessage>>> GetHistory(string userId)
    {
        try
        {
            var messages = await _chatRepository.GetAllAsync();
            var userMessages = messages.Where(m => m.UserId == userId).OrderByDescending(m => m.Timestamp);
            return Ok(userMessages);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
