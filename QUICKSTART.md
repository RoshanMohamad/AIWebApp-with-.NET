# Quick Setup Guide

## 1. Install Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/)
- Get [OpenAI API Key](https://platform.openai.com/api-keys)

## 2. Configure API Key

Edit `src/AIWebApp.API/appsettings.json`:

```json
{
  "OpenAI": {
    "ApiKey": "YOUR_ACTUAL_API_KEY_HERE"
  }
}
```

## 3. Start Backend

```bash
cd src\AIWebApp.API
dotnet restore
dotnet run
```

Visit: https://localhost:5001/swagger

## 4. Start Frontend

Open NEW terminal:

```bash
cd src\AIWebApp.UI
npm install
npm run dev
```

Visit: http://localhost:3000

## 5. Try It!

- **Chat**: Talk to AI assistant
- **Sentiment**: Analyze text emotions  
- **Documents**: Summarize text
- **Images**: Analyze photos

---

## Common Issues

**"OpenAI API Key not found"**
→ Add your API key to appsettings.json

**"Port already in use"**
→ Change port in launchSettings.json (API) or vite.config.ts (UI)

**CORS errors**
→ Check API is running on port 5000/5001

---

## Project Structure

```
├── src/
│   ├── AIWebApp.API/          → Backend API
│   ├── AIWebApp.Core/         → Domain models
│   ├── AIWebApp.Infrastructure/ → Database & AI
│   └── AIWebApp.UI/           → React frontend
└── README.md                  → Full documentation
```

For detailed docs, see [README.md](README.md)
