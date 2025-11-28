using System.Text.Json;
using AIWebApp.Core.Interfaces;
using AIWebApp.Core.Models;
using OpenAI.Chat;

namespace AIWebApp.Infrastructure.Services;

public class OpenAIService : IAIService
{
    private readonly ChatClient _chatClient;
    private readonly string _model;
    private readonly Dictionary<string, List<ChatMessage>> _conversationHistory;

    public OpenAIService(string apiKey, string model = "gpt-4o")
    {
        _chatClient = new ChatClient(model, apiKey);
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

            var messages = new List<OpenAI.Chat.ChatMessage>
            {
                new SystemChatMessage("You are a helpful AI assistant. Provide clear, accurate, and friendly responses.")
            };

            // Add conversation history
            foreach (var msg in _conversationHistory[sessionId])
            {
                messages.Add(new UserChatMessage(msg.Message));
                messages.Add(new AssistantChatMessage(msg.Response));
            }

            // Add current message
            messages.Add(new UserChatMessage(message));

            var completion = await _chatClient.CompleteChatAsync(messages);
            var response = completion.Value.Content[0].Text;

            // Store in history
            _conversationHistory[sessionId].Add(new ChatMessage
            {
                Message = message,
                Response = response,
                Timestamp = DateTime.UtcNow,
                SessionId = sessionId
            });

            // Keep only last 10 exchanges
            if (_conversationHistory[sessionId].Count > 10)
            {
                _conversationHistory[sessionId].RemoveAt(0);
            }

            return response;
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
            var messages = new List<OpenAI.Chat.ChatMessage>
            {
                new SystemChatMessage(@"You are a sentiment analysis expert. Analyze the sentiment of the given text and respond ONLY with a JSON object in this exact format:
{
    ""sentiment"": ""Positive"" or ""Negative"" or ""Neutral"",
    ""confidence"": 0.95,
    ""explanation"": ""Brief explanation of why""
}"),
                new UserChatMessage($"Analyze sentiment: {text}")
            };

            var completion = await _chatClient.CompleteChatAsync(messages);
            var response = completion.Value.Content[0].Text;

            // Parse JSON response
            var sentimentData = JsonSerializer.Deserialize<SentimentJsonResponse>(response);

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
            var base64Image = Convert.ToBase64String(imageData);
            var imageUrl = $"data:image/jpeg;base64,{base64Image}";

            var messages = new List<OpenAI.Chat.ChatMessage>
            {
                new SystemChatMessage(@"You are an image analysis expert. Analyze the image and respond ONLY with a JSON object in this exact format:
{
    ""description"": ""Detailed description of the image"",
    ""tags"": [""tag1"", ""tag2"", ""tag3""],
    ""objects"": [
        {""name"": ""object1"", ""confidence"": 0.95},
        {""name"": ""object2"", ""confidence"": 0.87}
    ]
}"),
                new UserChatMessage(
                    ChatMessageContentPart.CreateTextPart("Analyze this image"),
                    ChatMessageContentPart.CreateImagePart(new Uri(imageUrl))
                )
            };

            var completion = await _chatClient.CompleteChatAsync(messages);
            var response = completion.Value.Content[0].Text;

            // Parse JSON response
            var imageData = JsonSerializer.Deserialize<ImageJsonResponse>(response);

            return new ImageAnalysisResult
            {
                Description = imageData?.Description ?? "Unable to analyze image",
                Tags = imageData?.Tags ?? new List<string>(),
                Objects = imageData?.Objects?.Select(o => new DetectedObject
                {
                    Name = o.Name,
                    Confidence = o.Confidence
                }).ToList() ?? new List<DetectedObject>()
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
            var messages = new List<OpenAI.Chat.ChatMessage>
            {
                new SystemChatMessage(@"You are a document summarization expert. Create a concise summary and extract key points. Respond ONLY with a JSON object in this exact format:
{
    ""summary"": ""Brief summary of the document"",
    ""keyPoints"": [""Point 1"", ""Point 2"", ""Point 3""]
}"),
                new UserChatMessage($"Summarize this document:\n\n{text}")
            };

            var completion = await _chatClient.CompleteChatAsync(messages);
            var response = completion.Value.Content[0].Text;

            // Parse JSON response
            var summaryData = JsonSerializer.Deserialize<SummaryJsonResponse>(response);

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
        // Note: OpenAI's audio transcription requires using the Audio API
        // This is a placeholder implementation
        await Task.CompletedTask;
        return "Audio transcription requires OpenAI Whisper API integration. Feature coming soon.";
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
