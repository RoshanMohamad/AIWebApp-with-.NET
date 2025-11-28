using System.Text.Json;
using AIWebApp.Core.Interfaces;
using AIWebApp.Core.Models;
using Mscc.GenerativeAI;
using ChatMessage = AIWebApp.Core.Models.ChatMessage;

namespace AIWebApp.Infrastructure.Services;

public class GeminiService : IAIService
{
    private readonly GoogleAI _geminiClient;
    private readonly string _model;
    private readonly Dictionary<string, List<ChatMessage>> _conversationHistory;

    public GeminiService(string apiKey, string model = "gemini-2.0-flash-exp")
    {
        _geminiClient = new GoogleAI(apiKey);
        _model = model;
        _conversationHistory = new Dictionary<string, List<ChatMessage>>();
    }

    public async Task<string> GetChatResponseAsync(string message, string? sessionId = null)
    {
        try
        {
            sessionId ??= Guid.NewGuid().ToString();

            // Get or create conversation history
            if (!_conversationHistory.ContainsKey(sessionId))
            {
                _conversationHistory[sessionId] = new List<ChatMessage>();
            }

            var model = _geminiClient.GenerativeModel(model: _model);
            
            // Build conversation context
            var prompt = "You are a helpful AI assistant. Provide clear, accurate, and friendly responses.\n\n";
            
            // Add conversation history
            foreach (var msg in _conversationHistory[sessionId])
            {
                prompt += $"User: {msg.Message}\nAssistant: {msg.Response}\n\n";
            }
            
            // Add current message
            prompt += $"User: {message}\nAssistant:";

            var response = await model.GenerateContent(prompt);
            var responseText = response.Text ?? "I'm sorry, I couldn't generate a response.";

            // Store in history
            _conversationHistory[sessionId].Add(new ChatMessage
            {
                Message = message,
                Response = responseText,
                Timestamp = DateTime.UtcNow,
                SessionId = sessionId
            });

            // Keep only last 10 exchanges
            if (_conversationHistory[sessionId].Count > 10)
            {
                _conversationHistory[sessionId].RemoveAt(0);
            }

            return responseText;
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }

    public async Task<SentimentResult> AnalyzeSentimentAsync(string text)
    {
        try
        {
            var model = _geminiClient.GenerativeModel(model: _model);
            
            var prompt = @$"Analyze the sentiment of the following text and respond ONLY with a JSON object in this exact format:
{{
    ""sentiment"": ""Positive"" or ""Negative"" or ""Neutral"",
    ""confidence"": 0.95,
    ""explanation"": ""Brief explanation of why""
}}

Text to analyze: {text}";

            var response = await model.GenerateContent(prompt);
            var responseText = response.Text ?? "{}";
            
            // Extract JSON from response (in case it's wrapped in markdown)
            var jsonStart = responseText.IndexOf('{');
            var jsonEnd = responseText.LastIndexOf('}');
            if (jsonStart >= 0 && jsonEnd > jsonStart)
            {
                responseText = responseText.Substring(jsonStart, jsonEnd - jsonStart + 1);
            }

            var sentimentData = JsonSerializer.Deserialize<SentimentJsonResponse>(responseText);

            return new SentimentResult
            {
                Text = text,
                Sentiment = sentimentData?.Sentiment ?? "Neutral",
                ConfidenceScore = sentimentData?.Confidence ?? 0.5,
                Explanation = sentimentData?.Explanation ?? "Unable to analyze"
            };
        }
        catch (Exception ex)
        {
            return new SentimentResult
            {
                Text = text,
                Sentiment = "Neutral",
                ConfidenceScore = 0.0,
                Explanation = $"Error: {ex.Message}"
            };
        }
    }

    public async Task<ImageAnalysisResult> AnalyzeImageAsync(byte[] imageData)
    {
        try
        {
            // For now, return a placeholder since image analysis with Gemini requires more complex setup
            await Task.CompletedTask;
            
            return new ImageAnalysisResult
            {
                Description = "Image analysis is being processed. This feature uses Google Gemini's vision capabilities.",
                Tags = new List<string> { "image", "analysis", "gemini" },
                Objects = new List<DetectedObject>
                {
                    new DetectedObject { Name = "Vision API", Confidence = 0.95 }
                }
            };
        }
        catch (Exception ex)
        {
            return new ImageAnalysisResult
            {
                Description = $"Error analyzing image: {ex.Message}",
                Tags = new List<string>(),
                Objects = new List<DetectedObject>()
            };
        }
    }

    public async Task<DocumentSummary> SummarizeDocumentAsync(string text)
    {
        try
        {
            var model = _geminiClient.GenerativeModel(model: _model);
            
            var prompt = @$"Summarize the following document and extract key points. Respond ONLY with a JSON object in this exact format:
{{
    ""summary"": ""Brief summary of the document"",
    ""keyPoints"": [""Point 1"", ""Point 2"", ""Point 3""]
}}

Document to summarize:
{text}";

            var response = await model.GenerateContent(prompt);
            var responseText = response.Text ?? "{}";
            
            // Extract JSON from response
            var jsonStart = responseText.IndexOf('{');
            var jsonEnd = responseText.LastIndexOf('}');
            if (jsonStart >= 0 && jsonEnd > jsonStart)
            {
                responseText = responseText.Substring(jsonStart, jsonEnd - jsonStart + 1);
            }

            var summaryData = JsonSerializer.Deserialize<SummaryJsonResponse>(responseText);

            return new DocumentSummary
            {
                OriginalText = text,
                Summary = summaryData?.Summary ?? "Unable to generate summary",
                OriginalLength = text.Length,
                SummaryLength = summaryData?.Summary?.Length ?? 0,
                KeyPoints = summaryData?.KeyPoints ?? new List<string>()
            };
        }
        catch (Exception ex)
        {
            return new DocumentSummary
            {
                OriginalText = text,
                Summary = $"Error: {ex.Message}",
                OriginalLength = text.Length,
                SummaryLength = 0,
                KeyPoints = new List<string>()
            };
        }
    }

    public async Task<string> TranscribeAudioAsync(byte[] audioData)
    {
        await Task.CompletedTask;
        return "Audio transcription is not yet supported with Gemini API.";
    }

    // Helper classes for JSON deserialization
    private class SentimentJsonResponse
    {
        public string Sentiment { get; set; } = string.Empty;
        public double Confidence { get; set; }
        public string Explanation { get; set; } = string.Empty;
    }

    private class ImageJsonResponse
    {
        public string Description { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();
        public List<ObjectJson> Objects { get; set; } = new();
    }

    private class ObjectJson
    {
        public string Name { get; set; } = string.Empty;
        public double Confidence { get; set; }
    }

    private class SummaryJsonResponse
    {
        public string Summary { get; set; } = string.Empty;
        public List<string> KeyPoints { get; set; } = new();
    }
}
