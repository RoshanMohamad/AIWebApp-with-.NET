# 🚀 Automatic Deployment Setup Guide

## ✅ What's Configured

Your GitHub Actions workflow now includes automatic deployment that:
1. ✅ Builds your .NET API
2. ✅ Builds your React frontend
3. ✅ Creates a Docker image
4. ✅ Pushes to GitHub Container Registry
5. ✅ Deploys to your chosen platform

## 🎯 Choose Your Deployment Platform

### Option 1: Railway (Recommended - Easiest)

**Step 1: Connect Repository**
1. Go to https://railway.app
2. Sign in with GitHub
3. Click "New Project" → "Deploy from GitHub repo"
4. Select `AIWebApp-with-.NET`

**Step 2: Configure Environment**
Add this environment variable:
```
Gemini__ApiKey = your_gemini_api_key_here
```

**Step 3: Deploy**
- Railway will automatically detect the `railway.toml` file
- Click "Deploy"
- Your app will be live at: `https://your-app-name.up.railway.app`

**Step 4: (Optional) Add Automatic Deployment to GitHub Actions**
1. Get your Railway token from https://railway.app/account/tokens
2. In GitHub, go to Settings → Secrets → Actions
3. Add secret: `RAILWAY_TOKEN` = your token
4. In `.github/workflows/dotnet.yml`, uncomment the Railway deployment section

---

### Option 2: Render

**Step 1: Connect Repository**
1. Go to https://render.com
2. Click "New +" → "Blueprint"
3. Connect your GitHub repository
4. Render will detect `render.yaml`

**Step 2: Configure**
- Add environment variable: `Gemini__ApiKey`
- Click "Apply"

**Step 3: Get Deploy Hook (for GitHub Actions)**
1. In Render dashboard → Settings → Deploy Hook
2. Copy the webhook URL
3. In GitHub → Settings → Secrets → Actions
4. Add secret: `RENDER_DEPLOY_HOOK` = the URL
5. Uncomment the Render section in `.github/workflows/dotnet.yml`

---

### Option 3: Azure App Service

**Step 1: Create App Service**
```bash
az webapp create --resource-group MyResourceGroup --plan MyPlan --name aiwebapp-api --runtime "DOTNETCORE:8.0"
```

**Step 2: Get Publish Profile**
```bash
az webapp deployment list-publishing-profiles --resource-group MyResourceGroup --name aiwebapp-api --xml
```

**Step 3: Add to GitHub Secrets**
1. GitHub → Settings → Secrets → Actions
2. Add secret: `AZURE_WEBAPP_PUBLISH_PROFILE` = paste the XML
3. Uncomment Azure section in `.github/workflows/dotnet.yml`

---

## 🔐 Required GitHub Secrets

### Already Configured (No Action Needed)
- ✅ `GITHUB_TOKEN` - Automatically provided by GitHub

### Add These Secrets (Based on Platform)
Go to: https://github.com/RoshanMohamad/AIWebApp-with-.NET/settings/secrets/actions

**Core secret (required for app functionality):**
- `GEMINI_API_KEY` - your Gemini API key (used at runtime by the API). Add this in GitHub Secrets and to any hosting platform environment variables.

You can add the secret using the GitHub CLI (`gh`) locally:
```bash
gh auth login
gh secret set GEMINI_API_KEY --body "YOUR_GEMINI_KEY" --repo RoshanMohamad/AIWebApp-with-.NET
```

Or via the GitHub UI: Repository → Settings → Secrets & variables → Actions → New repository secret → `GEMINI_API_KEY`

**For Railway:**
- `RAILWAY_TOKEN` - Get from https://railway.app/account/tokens

**For Render:**
- `RENDER_DEPLOY_HOOK` - Get from Render dashboard

**For Azure:**
- `AZURE_WEBAPP_PUBLISH_PROFILE` - Get from Azure CLI

---

## 📦 Docker Image

Your app is automatically packaged as a Docker image and pushed to:
```
ghcr.io/roshanmohamad/aiwebapp:latest
```

You can pull and run it anywhere:
```bash
docker pull ghcr.io/roshanmohamad/aiwebapp:latest
docker run -p 5000:5000 -e Gemini__ApiKey=YOUR_KEY ghcr.io/roshanmohamad/aiwebapp:latest
```

---

## 🚀 Quick Start: Railway (No GitHub Actions Needed)

**Fastest way - Use Railway's native GitHub integration:**

1. Go to https://railway.app
2. New Project → Deploy from GitHub
3. Select your repo
4. Add environment variable: `Gemini__ApiKey`
5. Deploy
6. Done! ✨

Railway will automatically:
- Detect your Dockerfile
- Build your app
- Deploy on every push to main
- Give you a live URL

**No additional configuration needed!**

---

## 🔄 How Automatic Deployment Works

When you push to `main` or `backup-before-cleanup`:

1. GitHub Actions triggers
2. Builds your .NET app ✅
3. Builds your React app ✅
4. Creates Docker image ✅
5. Pushes to GitHub Container Registry ✅
6. Deploys to your platform (if configured) ✅

---

## ✅ Verify Deployment

Check GitHub Actions:
- https://github.com/RoshanMohamad/AIWebApp-with-.NET/actions

Check Docker Image:
- https://github.com/RoshanMohamad/aiwebapp-with-.net/pkgs/container/aiwebapp

---

## 🆘 Troubleshooting

**Build Fails:**
- Check GitHub Actions logs
- Verify all secrets are set
- Ensure Dockerfile is correct

**Deployment Fails:**
- Verify platform-specific secrets
- Check platform logs
- Ensure environment variables are set

**App Crashes:**
- Check `Gemini__ApiKey` is set
- Verify database connection
- Check platform logs
