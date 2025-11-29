using AIWebApp.Core.Interfaces;
using AIWebApp.Infrastructure.Data;
using AIWebApp.Infrastructure.Repositories;
using AIWebApp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

// Load .env file from root directory (3 levels up from bin/Debug/net10.0)
var currentDir = Directory.GetCurrentDirectory();
var rootPath = Path.GetFullPath(Path.Combine(currentDir, "..", "..", ".."));
var envPath = Path.Combine(rootPath, ".env");

// If running from src/AIWebApp.API, go up 2 more levels
if (!File.Exists(envPath))
{
    rootPath = Path.GetFullPath(Path.Combine(currentDir, "..", ".."));
    envPath = Path.Combine(rootPath, ".env");
}

if (File.Exists(envPath))
{
    DotNetEnv.Env.Load(envPath);
    Console.WriteLine($"✓ Loaded .env from: {envPath}");
}
else
{
    Console.WriteLine($"⚠ Warning: .env file not found at {envPath}");
}

var builder = WebApplication.CreateBuilder(args);

// Add environment variables to configuration
builder.Configuration.AddEnvironmentVariables();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:3001", "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repository Pattern
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// AI Service
var geminiKey = builder.Configuration["Gemini:ApiKey"];
if (string.IsNullOrEmpty(geminiKey))
{
    throw new Exception("Gemini API Key not found in configuration!");
}

builder.Services.AddSingleton<IAIService>(sp =>
    new GeminiService(geminiKey, builder.Configuration["Gemini:Model"] ?? "gemini-2.0-pro"));

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthorization();
app.MapControllers();

// Initialize Database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

app.Run();
