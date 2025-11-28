# 🤖 AI-Powered Web App in .NET

A modern, full-stack web application built with **.NET 8**, **ASP.NET Core**, and **React** that showcases powerful AI capabilities using **OpenAI GPT-4**.

## ✨ Features

### 1️⃣ **AI Chatbot**
- Interactive conversational AI powered by GPT-4
- Context-aware responses with conversation history
- Session management for continuous conversations

### 2️⃣ **Sentiment Analysis**
- Analyze text for emotional tone (Positive, Negative, Neutral)
- Confidence scoring
- Detailed explanations of sentiment reasoning

### 3️⃣ **Document Summarization**
- Upload text files or paste content directly
- AI-generated summaries with key points extraction
- Compression statistics

### 4️⃣ **Image Recognition**
- Upload images for AI analysis
- Detailed image descriptions
- Object detection with confidence scores
- Automatic tagging

## 🏗️ Architecture

```
AIWebApp/
├── src/
│   ├── AIWebApp.API/              # ASP.NET Core Web API
│   │   ├── Controllers/           # API Endpoints
│   │   ├── Program.cs             # Application startup
│   │   └── appsettings.json       # Configuration
│   │
│   ├── AIWebApp.Core/             # Domain Layer
│   │   ├── Models/                # Domain entities
│   │   ├── DTOs/                  # Data transfer objects
│   │   └── Interfaces/            # Contracts
│   │
│   ├── AIWebApp.Infrastructure/   # Infrastructure Layer
│   │   ├── Data/                  # Database context
│   │   ├── Repositories/          # Data access
│   │   └── Services/              # OpenAI integration
│   │
│   └── AIWebApp.UI/               # React Frontend
│       ├── src/
│       │   ├── pages/             # React pages
│       │   ├── api/               # API client
│       │   └── App.tsx            # Main app
│       └── package.json
│
└── AIWebApp.sln                   # Visual Studio Solution
```

## 🛠️ Tech Stack

### Backend
- **.NET 8** - Latest .NET framework
- **ASP.NET Core** - Web API framework
- **Entity Framework Core** - ORM for database access
- **SQLite** - Lightweight database (easily switch to SQL Server)
- **OpenAI SDK** - Official OpenAI library

### Frontend
- **React 18** - Modern UI library
- **TypeScript** - Type-safe JavaScript
- **Vite** - Fast build tool
- **Axios** - HTTP client
- **React Router** - Navigation
- **Lucide React** - Beautiful icons

### AI Integration
- **OpenAI GPT-4** - Text generation and analysis
- **GPT-4 Vision** - Image understanding

## 📋 Prerequisites

Before running this application, ensure you have:

1. **.NET 8 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
2. **Node.js 18+** - [Download here](https://nodejs.org/)
3. **OpenAI API Key** - [Get one here](https://platform.openai.com/api-keys)

## 🚀 Getting Started

### Step 1: Clone/Download the Project

Navigate to your project directory:
```bash
cd "d:\personal\.NET"
```

### Step 2: Configure OpenAI API Key

1. Open `src/AIWebApp.API/appsettings.json`
2. Replace `YOUR_OPENAI_API_KEY_HERE` with your actual OpenAI API key:

```json
{
  "OpenAI": {
    "ApiKey": "sk-proj-xxxxxxxxxxxxxxxxxxxxx",
    "Model": "gpt-4o"
  }
}
```

### Step 3: Run the Backend API

```bash
cd src\AIWebApp.API
dotnet restore
dotnet run
```

The API will start at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `https://localhost:5001/swagger`

### Step 4: Run the Frontend

Open a **new terminal** and run:

```bash
cd src\AIWebApp.UI
npm install
npm run dev
```

The React app will start at: `http://localhost:3000`

### Step 5: Open in Browser

Navigate to `http://localhost:3000` and start using the AI features!

## 📡 API Endpoints

### Chat
- `POST /api/chat` - Send a message to the chatbot
- `GET /api/chat/history/{userId}` - Get chat history

### Sentiment Analysis
- `POST /api/sentiment` - Analyze text sentiment

### Document
- `POST /api/document/summarize` - Summarize text
- `POST /api/document/upload` - Upload and summarize file

### Image
- `POST /api/image/analyze` - Analyze image

## 🗃️ Database

The application uses **SQLite** by default with the following schema:

- **ChatMessages** - Stores chat conversations
- **Users** - User accounts
- **AuditLogs** - Tracks all AI operations

Database file: `aiwebapp.db` (created automatically on first run)

### Switch to SQL Server (Optional)

1. Update `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=AIWebApp;Trusted_Connection=True;TrustServerCertificate=True"
}
```

2. Update `Program.cs`:
```csharp
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
```

## 🎨 UI Features

- **Modern gradient design** with smooth animations
- **Responsive layout** - works on desktop, tablet, and mobile
- **Dark mode** by default
- **Real-time updates** for chat interface
- **File upload** support for documents and images
- **Loading states** and error handling

## 🔒 Security Best Practices

1. **Never commit API keys** - Store in environment variables or Azure Key Vault
2. **Enable HTTPS** in production
3. **Add authentication** (e.g., JWT, OAuth)
4. **Rate limiting** to prevent API abuse
5. **Input validation** on all endpoints

## 📦 Deployment

### Deploy Backend (Azure App Service)

```bash
cd src\AIWebApp.API
dotnet publish -c Release -o ./publish
# Deploy ./publish folder to Azure App Service
```

### Deploy Frontend (Azure Static Web Apps / Vercel / Netlify)

```bash
cd src\AIWebApp.UI
npm run build
# Deploy ./dist folder
```

### Environment Variables

Set these in your production environment:
- `OpenAI__ApiKey` - Your OpenAI API key
- `ConnectionStrings__DefaultConnection` - Database connection string

## 🧪 Testing

Run backend tests:
```bash
dotnet test
```

## 🛠️ Development

### Adding New AI Features

1. Create a new method in `IAIService` interface
2. Implement in `OpenAIService`
3. Create a new controller endpoint
4. Add React page and API call

### Customizing the UI

- Edit `src/AIWebApp.UI/src/App.css` for global styles
- Modify pages in `src/AIWebApp.UI/src/pages/`
- Add new routes in `App.tsx`

## 📝 Sample API Requests

### Chat Request
```json
POST /api/chat
{
  "message": "What is artificial intelligence?",
  "userId": "user123",
  "sessionId": "abc-123"
}
```

### Sentiment Analysis Request
```json
POST /api/sentiment
{
  "text": "I absolutely love this product! It's amazing!"
}
```

## 🤝 Contributing

Contributions are welcome! Areas for improvement:

- [ ] Add user authentication (JWT/OAuth)
- [ ] Implement audio transcription (Whisper API)
- [ ] Add more AI models (Claude, Gemini)
- [ ] Create admin dashboard
- [ ] Add usage analytics
- [ ] Implement caching (Redis)
- [ ] Add rate limiting

## 📄 License

This project is open source and available under the MIT License.

## 🆘 Troubleshooting

### OpenAI API Errors
- Verify your API key is correct
- Check you have sufficient credits
- Ensure you're using a supported model (gpt-4o, gpt-4, gpt-3.5-turbo)

### CORS Errors
- Ensure the API CORS policy matches your frontend URL
- Check `Program.cs` CORS configuration

### Database Errors
- Delete `aiwebapp.db` and restart the API to recreate
- Verify connection string in `appsettings.json`

## 📧 Support

For issues or questions:
1. Check the troubleshooting section
2. Review API documentation at `/swagger`
3. Check OpenAI API status

## 🎯 Next Steps

After getting the app running, try:

1. **Customize the prompts** in `OpenAIService.cs` for better AI responses
2. **Add more features** like recommendation systems or smart search
3. **Deploy to production** using Azure, AWS, or other cloud providers
4. **Integrate with your own data** for domain-specific AI

---

Built with ❤️ using .NET 8, React, and OpenAI GPT-4
