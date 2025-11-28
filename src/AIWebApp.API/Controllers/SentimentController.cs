using AIWebApp.Core.DTOs;
using AIWebApp.Core.Interfaces;
using AIWebApp.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace AIWebApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SentimentController : ControllerBase
{
    private readonly IAIService _aiService;
    private readonly IRepository<AuditLog> _auditRepository;

    public SentimentController(IAIService aiService, IRepository<AuditLog> auditRepository)
    {
        _aiService = aiService;
        _auditRepository = auditRepository;
    }

    [HttpPost]
    public async Task<ActionResult<SentimentResponse>> AnalyzeSentiment([FromBody] SentimentRequest request)
    {
        try
        {
            var result = await _aiService.AnalyzeSentimentAsync(request.Text);

            // Log action
            await _auditRepository.AddAsync(new AuditLog
            {
                UserId = "anonymous",
                Action = "AnalyzeSentiment",
                Feature = "SentimentAnalysis",
                Timestamp = DateTime.UtcNow,
                Details = $"Text length: {request.Text.Length}"
            });

            return Ok(new SentimentResponse
            {
                Sentiment = result.Sentiment,
                Confidence = result.ConfidenceScore,
                Explanation = result.Explanation ?? ""
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
