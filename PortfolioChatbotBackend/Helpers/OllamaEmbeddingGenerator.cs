using Microsoft.KernelMemory;
using Microsoft.KernelMemory.AI;
using PortfolioChatbotBackend.Services;

namespace PortfolioChatbotBackend.Helpers
{
    public class OllamaEmbeddingGenerator : ITextEmbeddingGenerator
    {
        private readonly OllamaService _ollamaService;
        private readonly ILogger _logger;

        public OllamaEmbeddingGenerator(OllamaService ollamaService, ILogger logger)
        {
            _ollamaService = ollamaService;
            _logger = logger;
        }

        public int MaxTokens => 2256;

        public int CountTokens(string text)
        {
            // Simple estimate: ~4 chars per token on average
            return text.Length / 4;
        }

        public async Task<Embedding> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation($"Generating embedding for text: {text.Substring(0, Math.Min(50, text.Length))}...");

                // Use our OllamaService to generate the embedding
                var embedding = await _ollamaService.GenerateEmbeddingsAsync(text);

                return new Embedding(embedding);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating embedding");
                throw;
            }
        }

        public IReadOnlyList<string> GetTokens(string text)
        {
            // Very simple token splitting - this is just a rough approximation
            // Split by space and punctuation
            return text.Split(new[] { ' ', '.', ',', '!', '?', ';', ':', '\n', '\r', '\t' },
                StringSplitOptions.RemoveEmptyEntries);
        }

       
    }
}