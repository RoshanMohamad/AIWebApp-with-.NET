# AI Features Implementation Guide

## How Each Feature Works

### 1. AI Chatbot (GPT-4)

**Backend**: `OpenAIService.GetChatResponseAsync()`
- Maintains conversation history per session
- Uses system prompt to define AI behavior
- Stores last 10 exchanges for context

**Customization**:
```csharp
// Change the system prompt in OpenAIService.cs
new SystemChatMessage("You are a helpful AI assistant...")
```

**Frontend**: `ChatPage.tsx`
- Real-time message display
- Session-based conversations
- Auto-scroll to latest message

---

### 2. Sentiment Analysis

**Backend**: `OpenAIService.AnalyzeSentimentAsync()`
- Uses GPT-4 to analyze emotional tone
- Returns: Positive, Negative, or Neutral
- Includes confidence score (0-1)
- Provides reasoning/explanation

**How it works**:
1. Sends text to GPT-4 with specific JSON format request
2. AI analyzes sentiment and explains why
3. Returns structured result

**Frontend**: `SentimentPage.tsx`
- Color-coded badges (green/red/gray)
- Confidence percentage display
- Explanation of sentiment

---

### 3. Document Summarization

**Backend**: `OpenAIService.SummarizeDocumentAsync()`
- Generates concise summaries
- Extracts key points
- Calculates compression ratio

**Features**:
- Upload .txt files
- Paste text directly
- View original vs summary length

**Use Cases**:
- Summarize articles
- Extract meeting notes
- Condense reports

**Frontend**: `DocumentPage.tsx`
- File upload support
- Direct text input
- Key points as bullet list

---

### 4. Image Recognition

**Backend**: `OpenAIService.AnalyzeImageAsync()`
- Uses GPT-4 Vision (gpt-4o model)
- Describes image contents
- Detects objects with confidence scores
- Generates relevant tags

**How it works**:
1. Converts image to base64
2. Sends to GPT-4 Vision API
3. AI returns description, objects, and tags

**Frontend**: `ImagePage.tsx`
- Image preview before analysis
- Detailed description
- Object list with confidence
- Tag cloud

---

## Advanced Customization

### Change AI Model

Edit `appsettings.json`:
```json
{
  "OpenAI": {
    "Model": "gpt-4o"  // or "gpt-4-turbo", "gpt-3.5-turbo"
  }
}
```

### Adjust Temperature (Creativity)

In `OpenAIService.cs`, add:
```csharp
var options = new ChatCompletionOptions
{
    Temperature = 0.7f  // 0 = deterministic, 1 = creative
};
```

### Add Rate Limiting

Install: `AspNetCoreRateLimit`
```bash
dotnet add package AspNetCoreRateLimit
```

### Add Caching

For repeated queries, add Redis caching:
```bash
dotnet add package Microsoft.Extensions.Caching.StackExchangeRedis
```

---

## Database Schema

### ChatMessage
- Stores all chat conversations
- Links messages to users and sessions
- Enables history retrieval

### AuditLog
- Tracks all AI operations
- Useful for monitoring usage
- Helps debug issues

### User
- Basic user tracking
- Can extend for authentication

---

## API Response Times

Typical response times:
- **Chat**: 2-5 seconds
- **Sentiment**: 1-3 seconds  
- **Summary**: 3-7 seconds
- **Image**: 4-8 seconds

Times vary based on:
- OpenAI API load
- Input size
- Model selected

---

## Cost Management

OpenAI pricing (as of 2024):

**GPT-4o** (recommended):
- Input: $2.50 / 1M tokens
- Output: $10.00 / 1M tokens

**GPT-4 Turbo**:
- Input: $10 / 1M tokens
- Output: $30 / 1M tokens

**Tips to reduce costs**:
1. Use gpt-3.5-turbo for simple tasks
2. Implement response caching
3. Add request rate limiting
4. Set max token limits

---

## Security Considerations

### Protect API Key
```bash
# Use environment variable
$env:OpenAI__ApiKey = "sk-..."

# Or Azure Key Vault
dotnet add package Azure.Security.KeyVault.Secrets
```

### Add Authentication
```bash
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

### Input Validation
Already implemented in controllers:
- File size limits
- Text length checks
- MIME type validation

---

## Extending Features

### Add New AI Feature

1. **Interface** (`IAIService`):
```csharp
Task<TranslationResult> TranslateTextAsync(string text, string targetLang);
```

2. **Implementation** (`OpenAIService`):
```csharp
public async Task<TranslationResult> TranslateTextAsync(string text, string targetLang)
{
    // Your implementation
}
```

3. **Controller**:
```csharp
[HttpPost("translate")]
public async Task<ActionResult<TranslationResult>> Translate([FromBody] TranslateRequest request)
{
    var result = await _aiService.TranslateTextAsync(request.Text, request.Language);
    return Ok(result);
}
```

4. **Frontend** - Create new page/component

---

## Production Checklist

- [ ] Store API key in environment variables
- [ ] Enable HTTPS redirect
- [ ] Add authentication/authorization
- [ ] Implement rate limiting
- [ ] Add response caching
- [ ] Set up logging (Serilog/Application Insights)
- [ ] Configure database backups
- [ ] Add error tracking (Sentry)
- [ ] Enable CORS for specific domains only
- [ ] Add health check endpoints
- [ ] Set up CI/CD pipeline
- [ ] Monitor OpenAI API usage

---

## Resources

- [OpenAI API Documentation](https://platform.openai.com/docs)
- [.NET 8 Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [React Documentation](https://react.dev/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
