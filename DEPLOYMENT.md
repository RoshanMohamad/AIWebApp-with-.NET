# 🚀 Deployment Guide

## Prerequisites

- Git installed
- GitHub account
- Docker (optional, for containerized deployment)
- .NET 8 SDK (for local deployment)
- Node.js 18+ (for local deployment)

## 📋 Quick Start

### 1. Push to GitHub

Your repository is already connected to GitHub:
```bash
git remote -v
# origin  https://github.com/RoshanMohamad/AIWebApp-with-.NET.git
```

**To push your changes:**

```bash
# Check current status
git status

# Add all changes
git add .

# Commit with a meaningful message
git commit -m "Add deployment configuration and GitHub Actions"

# Push to your current branch
git push origin backup-before-cleanup

# Or push to main branch
git checkout main
git merge backup-before-cleanup
git push origin main
```

### 2. Setup Secrets in GitHub

Go to your GitHub repository → Settings → Secrets and variables → Actions

Add the following secrets:
- `GEMINI_API_KEY`: Your Gemini API key

### 3. Enable GitHub Actions

- Go to the "Actions" tab in your GitHub repository
- Enable workflows if prompted
- The `.github/workflows/dotnet.yml` will automatically run on push

## 🐳 Docker Deployment

### Local Docker Build

```bash
# Build and run with Docker Compose
docker-compose up -d

# Or build individual services
docker build -t aiwebapp-api .
docker build -t aiwebapp-ui -f src/AIWebApp.UI/Dockerfile.ui src/AIWebApp.UI

# Run containers
docker run -d -p 5000:5000 -e Gemini__ApiKey=YOUR_KEY aiwebapp-api
docker run -d -p 3000:80 aiwebapp-ui
```

### Environment Variables

Create a `.env` file for Docker Compose:
```env
GEMINI_API_KEY=your_gemini_api_key_here
```

## ☁️ Cloud Deployment Options

### Option 1: Azure App Service

1. **Create App Service:**
```bash
az webapp create --resource-group MyResourceGroup --plan MyPlan --name aiwebapp-api --runtime "DOTNETCORE:8.0"
```

2. **Deploy:**
```bash
dotnet publish -c Release
cd src/AIWebApp.API/bin/Release/net8.0/publish
zip -r publish.zip .
az webapp deployment source config-zip --resource-group MyResourceGroup --name aiwebapp-api --src publish.zip
```

3. **Set Environment Variables:**
```bash
az webapp config appsettings set --resource-group MyResourceGroup --name aiwebapp-api --settings Gemini__ApiKey=YOUR_KEY
```

### Option 2: Heroku

1. **Install Heroku CLI and login:**
```bash
heroku login
```

2. **Create app:**
```bash
heroku create aiwebapp-api
```

3. **Add buildpack:**
```bash
heroku buildpacks:set heroku/dotnet
```

4. **Set environment variables:**
```bash
heroku config:set Gemini__ApiKey=YOUR_KEY
```

5. **Deploy:**
```bash
git push heroku main
```

### Option 3: Railway

1. Go to [Railway.app](https://railway.app)
2. Connect your GitHub repository
3. Select the repository
4. Add environment variables in the Railway dashboard
5. Railway will automatically deploy from your `main` branch

### Option 4: Render

1. Go to [Render.com](https://render.com)
2. Create New → Web Service
3. Connect your GitHub repository
4. Configure:
   - Build Command: `dotnet publish -c Release -o out`
   - Start Command: `dotnet out/AIWebApp.API.dll`
5. Add environment variables
6. Deploy

### Option 5: DigitalOcean App Platform

1. Go to DigitalOcean App Platform
2. Create App → GitHub
3. Select your repository
4. Configure build settings
5. Add environment variables
6. Deploy

## 🔒 Security Checklist

Before deploying:

- ✅ `.env` file is in `.gitignore`
- ✅ API keys are stored in environment variables
- ✅ `appsettings.json` contains no secrets
- ✅ CORS is configured for your production domain
- ✅ HTTPS is enabled in production
- ✅ Database connection strings are secured

## 📊 Monitoring

After deployment, monitor:
- Application logs
- Error rates
- API response times
- Database performance

## 🔄 Continuous Deployment

GitHub Actions will automatically:
1. Build and test on every push
2. Create artifacts for deployment
3. Run security checks

To enable automatic deployment to cloud providers, add their deployment actions to `.github/workflows/dotnet.yml`.

## 🆘 Troubleshooting

### Build Fails
- Check .NET SDK version (should be 8.0)
- Verify all NuGet packages restore correctly
- Check for syntax errors

### Runtime Errors
- Verify environment variables are set
- Check database connection string
- Review application logs
- Ensure Gemini API key is valid

### CORS Issues
- Update CORS origins in `Program.cs`
- Add your production domain to allowed origins

## 📚 Additional Resources

- [ASP.NET Core Deployment](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/)
- [Docker Documentation](https://docs.docker.com/)
- [GitHub Actions](https://docs.github.com/en/actions)
