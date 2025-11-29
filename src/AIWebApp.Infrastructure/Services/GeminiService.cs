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

    public async Task<ImageAnalysisResult> AnalyzeImageAsync(byte[] imageBytes)
    {
        try
        {
            // Use the configured generative model
            var model = _geminiClient.GenerativeModel(model: _model);
            
            // Convert image to base64
            var base64Image = Convert.ToBase64String(imageBytes);
            
            // Determine image MIME type (default to JPEG if unknown)
            var mimeType = "image/jpeg";
            if (imageBytes.Length > 2)
            {
                // PNG signature
                if (imageBytes[0] == 0x89 && imageBytes[1] == 0x50)
                    mimeType = "image/png";
                // GIF signature
                else if (imageBytes[0] == 0x47 && imageBytes[1] == 0x49)
                    mimeType = "image/gif";
                // WebP signature
                else if (imageBytes[0] == 0x52 && imageBytes[1] == 0x49)
                    mimeType = "image/webp";
            }
            
            var prompt = @"Analyze this image in detail and provide a comprehensive response in JSON format with the following structure:
{
    ""description"": ""A detailed, comprehensive description of what you see in the image. Include colors, objects, people, activities, mood, composition, and any notable features. Be descriptive and thorough (3-5 sentences)."",
    ""tags"": [""list"", ""of"", ""relevant"", ""keywords"", ""and"", ""tags""],
    ""objects"": [
        {""name"": ""object1"", ""confidence"": 0.95},
        {""name"": ""object2"", ""confidence"": 0.87}
    ],
    ""scene"": ""The overall scene or setting (indoor/outdoor, location type, time of day if visible)"",
    ""colors"": [""dominant"", ""color"", ""palette""],
    ""mood"": ""The emotional tone or atmosphere of the image"",
    ""details"": ""Any interesting or unique details worth noting""
}

Provide accurate and detailed analysis. For confidence scores, use values between 0.70 and 0.99 based on how certain you are about each object.";

            // Create request with text and image
            var request = new GenerateContentRequest
            {
                Contents = new List<Content>
                {
                    new Content
                    {
                        Parts = new List<IPart>
                        {
                            new TextData { Text = prompt },
                            new InlineData { MimeType = mimeType, Data = base64Image }
                        }
                    }
                }
            };
            
            var response = await model.GenerateContent(request);
            var responseText = response.Text ?? "{}";
            
            // Extract JSON from response
            var jsonStart = responseText.IndexOf('{');
            var jsonEnd = responseText.LastIndexOf('}');
            if (jsonStart >= 0 && jsonEnd > jsonStart)
            {
                responseText = responseText.Substring(jsonStart, jsonEnd - jsonStart + 1);
            }

            var imageData = JsonSerializer.Deserialize<ImageJsonResponse>(responseText);

            // Build a comprehensive description
            var fullDescription = imageData?.Description ?? "Unable to analyze image";
            
            if (!string.IsNullOrEmpty(imageData?.Scene))
            {
                fullDescription += $"\n\nScene: {imageData.Scene}";
            }
            
            if (imageData?.Colors?.Count > 0)
            {
                fullDescription += $"\n\nColor Palette: {string.Join(", ", imageData.Colors)}";
            }
            
            if (!string.IsNullOrEmpty(imageData?.Mood))
            {
                fullDescription += $"\n\nMood/Atmosphere: {imageData.Mood}";
            }
            
            if (!string.IsNullOrEmpty(imageData?.Details))
            {
                fullDescription += $"\n\nNotable Details: {imageData.Details}";
            }

            return new ImageAnalysisResult
            {
                Description = fullDescription,
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
                Description = $"Error analyzing image: {ex.Message}. Please ensure the image is in a supported format (JPEG, PNG, GIF, WebP) and try again.",
                Tags = new List<string> { "error" },
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
        public string Scene { get; set; } = string.Empty;
        public List<string> Colors { get; set; } = new();
        public string Mood { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
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
