using AIWebApp.Core.Interfaces;
using AIWebApp.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace AIWebApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImageController : ControllerBase
{
    private readonly IAIService _aiService;
    private readonly IRepository<AuditLog> _auditRepository;

    public ImageController(IAIService aiService, IRepository<AuditLog> auditRepository)
    {
        _aiService = aiService;
        _auditRepository = auditRepository;
    }

    [HttpPost("analyze")]
    public async Task<ActionResult<ImageAnalysisResult>> AnalyzeImage(IFormFile image)
    {
        try
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest(new { error = "No image provided" });
            }

            // Read image bytes
            using var memoryStream = new MemoryStream();
            await image.CopyToAsync(memoryStream);
            var imageBytes = memoryStream.ToArray();

            var result = await _aiService.AnalyzeImageAsync(imageBytes);

            // Log action
            await _auditRepository.AddAsync(new AuditLog
            {
                UserId = "anonymous",
                Action = "AnalyzeImage",
                Feature = "ImageRecognition",
                Timestamp = DateTime.UtcNow,
                Details = $"Image size: {image.Length} bytes"
            });

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
