using Microsoft.KernelMemory;

namespace PortfolioChatbotBackend.Services
{
    public class ChatBackendService
    {
        private readonly OllamaService _ollamaService;
        private readonly IKernelMemory _memory;
        private readonly PortfolioDataStore _dataStore;
        private readonly ILogger<ChatBackendService> _logger;
        private readonly int _maxContextChars = 2000; // Limit context size

        public ChatBackendService(
            OllamaService ollamaService,
            IKernelMemory memory,
            PortfolioDataStore dataStore,
            ILogger<ChatBackendService> logger)
        {
            _ollamaService = ollamaService;
            _memory = memory;
            _dataStore = dataStore;
            _logger = logger;
        }

        public async Task<string> GetBotResponseAsync(string userQuery)
        {
            if (string.IsNullOrWhiteSpace(userQuery))
            {
                return "Please type a message.";
            }

            try
            {
                _logger.LogInformation("Processing query: {Query}", userQuery);

                // First try direct memory Ask for a semantically appropriate answer
                try
                {
                    var answer = await _memory.AskAsync(userQuery);
                    if (!string.IsNullOrEmpty(answer.Result))
                    {
                        _logger.LogInformation("Generated answer from memory: {Answer}", answer.Result);
                        return answer.Result;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error getting answer from memory, falling back to direct Ollama completion");

                    // On error, try the simplest direct approach
                    var prompt = $@"You are Abdalla's portfolio website assistant. Answer the following question:
                    
Question: {userQuery}

Answer:";

                    // Direct Ollama call with a simple prompt
                    var directResponse = await _ollamaService.GenerateCompletionAsync(prompt);
                    return directResponse;
                }

                // If we reached here, AskAsync returned empty but didn't error
                // Try a direct approach with Ollama using a general portfolio context
                var generalPrompt = $@"You are an assistant for Abdalla's portfolio website. 
Abdalla Elkilany is a Computer Engineer and Full-Stack .NET & Angular Developer. 
He has experience with .NET Core, ASP.NET Core, Angular, C#, TypeScript, and SQL Server.
He has worked on projects including real-time order tracking, parking management systems,
smart surveillance systems, and WPF applications.

Answer the following question based on this information:

Question: {userQuery}

Answer:";

                var response = await _ollamaService.GenerateCompletionAsync(generalPrompt);
                _logger.LogInformation("Generated response using simplified context");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating response");
                return "I'm sorry, I encountered an error while processing your request. Please try a simpler question or try again later.";
            }
        }
    }
}