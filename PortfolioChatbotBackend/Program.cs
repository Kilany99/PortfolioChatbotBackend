using Microsoft.SemanticKernel;
using Microsoft.KernelMemory;
using PortfolioChatbotBackend.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.KernelMemory.AI.Ollama;
using PortfolioChatbotBackend.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- Configure HttpClient for direct Ollama API calls ---
builder.Services.AddHttpClient("OllamaClient", client =>
{
    var endpoint = builder.Configuration["SemanticKernel:Ollama:Endpoint"] ?? "http://localhost:11434";
    client.BaseAddress = new Uri(endpoint);
});

// --- Add a custom Ollama service ---
builder.Services.AddSingleton<OllamaService>();

// --- Add a data store for direct content access ---
builder.Services.AddSingleton<PortfolioDataStore>();

// --- Configure Kernel Memory with custom text/embedding generators ---
builder.Services.AddSingleton<IKernelMemory>(sp =>
{
    var ollamaService = sp.GetRequiredService<OllamaService>();
    var logger = sp.GetRequiredService<ILogger<OllamaService>>();
    var dataStore = sp.GetRequiredService<PortfolioDataStore>();

    try
    {
        // Create the memory with all required components
        var memoryBuilder = new KernelMemoryBuilder();

        // 1. Set up storage
        memoryBuilder.WithSimpleVectorDb();

        // 2. Set up custom embedding generator that uses our OllamaService
        memoryBuilder.WithCustomEmbeddingGenerator(new OllamaEmbeddingGenerator(ollamaService, logger));

        // 3. Set up custom text generator that uses our OllamaService
        memoryBuilder.WithCustomTextGenerator(new PortfolioChatbotBackend.Helpers.OllamaTextGenerator(ollamaService,logger));

        logger.LogInformation("Building memory with Ollama integrations");
        var memory = memoryBuilder.Build();
        logger.LogInformation("Successfully built memory");

        return memory;
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to build memory");
        throw;
    }
});

// --- Add your Chat Backend Service ---
builder.Services.AddScoped<ChatBackendService>();

// --- Configure CORS ---
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

// --- Trigger Data Ingestion on Startup ---
using (var scope = app.Services.CreateScope())
{
    try
    {
        var memory = scope.ServiceProvider.GetRequiredService<IKernelMemory>();
        var dataStore = scope.ServiceProvider.GetRequiredService<PortfolioDataStore>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<PortfolioDataIngestionService>>();

        logger.LogInformation("Starting portfolio data ingestion...");
        var ingestionService = new PortfolioDataIngestionService(memory, dataStore, logger);
        await ingestionService.IngestDataAsync();
        logger.LogInformation("Portfolio data ingestion complete.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error during data ingestion: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
        // Log the error but continue application startup
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();
