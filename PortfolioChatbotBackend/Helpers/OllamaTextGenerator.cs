using Microsoft.KernelMemory;
using Microsoft.KernelMemory.AI;
using PortfolioChatbotBackend.Services;

namespace PortfolioChatbotBackend.Helpers
{
    public class OllamaTextGenerator : ITextGenerator
    {
        private readonly OllamaService _ollamaService;
        private readonly ILogger _logger;

        public OllamaTextGenerator(OllamaService ollamaService, ILogger logger)
        {
            _ollamaService = ollamaService;
            _logger = logger;
        }

        // Provide reasonable estimates for these properties/methods
        public int MaxTokenTotal => 4096; // Reasonable default for most LLMs

        public int CountTokens(string text)
        {
            // Simple estimate: ~4 chars per token on average
            return text.Length / 4;
        }

        public async Task<string> GenerateTextAsync(string prompt, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation($"Generating text for prompt: {prompt.Substring(0, Math.Min(50, prompt.Length))}...");

                // Use our OllamaService to generate the text
                var response = await _ollamaService.GenerateCompletionAsync(prompt);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating text");
                throw;
            }
        }

        public async IAsyncEnumerable<GeneratedTextContent> GenerateTextAsync(
           string prompt,
           TextGenerationOptions options,
           [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            // Simple implementation that doesn't actually stream  
            var text = await GenerateTextAsync(prompt, cancellationToken);

            yield return new GeneratedTextContent(
                text, // Pass the required 'text' parameter  
                null  // Pass null for the optional 'TokenUsage?' parameter  
            );
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