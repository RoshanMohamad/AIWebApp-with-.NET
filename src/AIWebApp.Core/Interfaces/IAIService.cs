using AIWebApp.Core.Models;

namespace AIWebApp.Core.Interfaces;

public interface IAIService
{
    Task<string> GetChatResponseAsync(string message, string? sessionId = null);
    Task<SentimentResult> AnalyzeSentimentAsync(string text);
    Task<ImageAnalysisResult> AnalyzeImageAsync(byte[] imageData);
    Task<DocumentSummary> SummarizeDocumentAsync(string text);
    Task<string> TranscribeAudioAsync(byte[] audioData);
}
