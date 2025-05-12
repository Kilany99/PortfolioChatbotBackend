using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using PortfolioChatbotBackend.Services;
using PortfolioChatbotBackend.Models;

namespace PortfolioChatbotBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly ChatBackendService _chatService;
        private readonly ILogger<ChatController> _logger;

        public ChatController(ChatBackendService chatService, ILogger<ChatController> logger)
        {
            _chatService = chatService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<ChatResponse>> PostMessage([FromBody] ChatRequest request)
        {
            _logger.LogInformation($"Received message: {request?.Message}");

            if (string.IsNullOrWhiteSpace(request?.Message))
            {
                _logger.LogWarning("Received empty message.");
                return BadRequest("Message cannot be empty.");
            }

            try
            {
                // Call the backend service to get the AI response
                var botResponse = await _chatService.GetBotResponseAsync(request.Message);

                _logger.LogInformation($"Sending response: {botResponse}");

                // Return the response in the specified format
                return Ok(new ChatResponse { BotMessage = botResponse });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing chat message.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
    }
}