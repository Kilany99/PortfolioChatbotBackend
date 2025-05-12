using System.Text;
using System.Text.Json;

namespace PortfolioChatbotBackend.Services
{
    public class OllamaService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<OllamaService> _logger;
        private readonly string _completionModel;
        private readonly string _embeddingModel;
        private readonly int _maxPromptSize = 4000; // Maximum size of prompt to prevent 500 errors

        public OllamaService(IHttpClientFactory httpClientFactory, IConfiguration config, ILogger<OllamaService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _completionModel = config["SemanticKernel:Ollama:ModelName"] ?? "llama3";
            _embeddingModel = config["SemanticKernel:Ollama:EmbeddingModelName"] ?? "nomic-embed-text";
        }

        public async Task<string> GenerateCompletionAsync(string prompt)
        {
            try
            {
                // Truncate prompt if it's too large
                if (prompt.Length > _maxPromptSize)
                {
                    _logger.LogWarning($"Prompt is too large ({prompt.Length} chars). Truncating to {_maxPromptSize} chars.");

                    // Keep the beginning and end of the prompt, cutting out the middle
                    var start = prompt.Substring(0, 1000); // Keep first part
                    var end = prompt.Substring(prompt.Length - (_maxPromptSize - 1000)); // Keep last part
                    prompt = start + "\n...[content truncated for length]...\n" + end;
                }

                var client = _httpClientFactory.CreateClient("OllamaClient");

                var request = new
                {
                    model = _completionModel,
                    prompt = prompt,
                    stream = false
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(request),
                    Encoding.UTF8,
                    "application/json");

                var response = await client.PostAsync("/api/generate", content);

                // Enhanced error handling
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Ollama error: Status {response.StatusCode}, Content: {errorContent}");

                    // Fall back to a simpler approach
                    if (prompt.Length > 2000)
                    {
                        _logger.LogWarning("Retrying with a much shorter prompt");
                        return await GenerateCompletionAsync("Summarize this information: " + prompt.Substring(0, 1000));
                    }

                    return $"I encountered an error while processing your request. The model may be overloaded or the request too complex. Please try again with a simpler question.";
                }

                var result = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonDocument.Parse(result);

                // Extract the response from the JSON
                return jsonResponse.RootElement.GetProperty("response").GetString() ?? "";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating completion with Ollama");
                return $"I encountered an error while processing your request: {ex.Message.Substring(0, Math.Min(ex.Message.Length, 100))}. Please try again later.";
            }
        }

        public async Task<float[]> GenerateEmbeddingsAsync(string text)
        {
            try
            {
                // Truncate text if it's too large
                if (text.Length > _maxPromptSize)
                {
                    _logger.LogWarning($"Text for embedding is too large ({text.Length} chars). Truncating to {_maxPromptSize} chars.");
                    text = text.Substring(0, _maxPromptSize);
                }

                var client = _httpClientFactory.CreateClient("OllamaClient");

                var request = new
                {
                    model = _embeddingModel,
                    prompt = text
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(request),
                    Encoding.UTF8,
                    "application/json");

                var response = await client.PostAsync("/api/embeddings", content);

                // Enhanced error handling
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Ollama embedding error: Status {response.StatusCode}, Content: {errorContent}");

                    // Return a fallback embedding (zeros)
                    return new float[1536]; // Standard embedding size
                }

                var result = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonDocument.Parse(result);

                // Extract the embeddings from the JSON
                var embeddings = jsonResponse.RootElement.GetProperty("embedding");
                var embeddingList = new List<float>();

                foreach (var embedding in embeddings.EnumerateArray())
                {
                    embeddingList.Add(embedding.GetSingle());
                }

                return embeddingList.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating embeddings with Ollama");
                // Return fallback embedding
                return new float[1536]; // Standard embedding size
            }
        }
    }
}