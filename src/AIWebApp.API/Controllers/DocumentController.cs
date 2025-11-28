using AIWebApp.Core.Interfaces;
using AIWebApp.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace AIWebApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentController : ControllerBase
{
    private readonly IAIService _aiService;
    private readonly IRepository<AuditLog> _auditRepository;

    public DocumentController(IAIService aiService, IRepository<AuditLog> auditRepository)
    {
        _aiService = aiService;
        _auditRepository = auditRepository;
    }

    [HttpPost("summarize")]
    public async Task<ActionResult<DocumentSummary>> SummarizeDocument([FromBody] SummarizeRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Text))
            {
                return BadRequest(new { error = "Text is required" });
            }

            var result = await _aiService.SummarizeDocumentAsync(request.Text);

            // Log action
            await _auditRepository.AddAsync(new AuditLog
            {
                UserId = "anonymous",
                Action = "SummarizeDocument",
                Feature = "DocumentSummarization",
                Timestamp = DateTime.UtcNow,
                Details = $"Document length: {request.Text.Length} chars"
            });

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("upload")]
    public async Task<ActionResult<DocumentSummary>> UploadAndSummarize(IFormFile document)
    {
        try
        {
            if (document == null || document.Length == 0)
            {
                return BadRequest(new { error = "No document provided" });
            }

            // Read document text (supports .txt files)
            using var reader = new StreamReader(document.OpenReadStream());
            var text = await reader.ReadToEndAsync();

            var result = await _aiService.SummarizeDocumentAsync(text);

            // Log action
            await _auditRepository.AddAsync(new AuditLog
            {
                UserId = "anonymous",
                Action = "UploadDocument",
                Feature = "DocumentSummarization",
                Timestamp = DateTime.UtcNow,
                Details = $"File: {document.FileName}"
            });

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}

public class SummarizeRequest
{
    public string Text { get; set; } = string.Empty;
}
