# Multi-stage build for .NET API
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY ["AIWebApp.sln", "./"]

# Copy csproj files and restore dependencies
COPY ["src/AIWebApp.API/AIWebApp.API.csproj", "src/AIWebApp.API/"]
COPY ["src/AIWebApp.Core/AIWebApp.Core.csproj", "src/AIWebApp.Core/"]
COPY ["src/AIWebApp.Infrastructure/AIWebApp.Infrastructure.csproj", "src/AIWebApp.Infrastructure/"]
RUN dotnet restore "src/AIWebApp.API/AIWebApp.API.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/src/AIWebApp.API"
RUN dotnet build "AIWebApp.API.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "AIWebApp.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AIWebApp.API.dll"]
