using Firmness.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Firmness.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IAiChatService _aiChatService;
    private readonly ILogger<ChatController> _logger;

    public ChatController(IAiChatService aiChatService, ILogger<ChatController> logger)
    {
        _aiChatService = aiChatService;
        _logger = logger;
    }

    [HttpPost("message")]
    public async Task<IActionResult> SendMessage([FromBody] ChatRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new { error = "Message cannot be empty" });
        }

        try
        {
            _logger.LogInformation("Received chat message: {Message}", request.Message);

            // Convert request history to service format
            var conversationHistory = request.ConversationHistory?
                .Select(m => new ChatMessage 
                { 
                    Role = m.IsUser ? "user" : "assistant", 
                    Content = m.Text 
                })
                .ToList();

            var aiResponse = await _aiChatService.GetAiResponseAsync(request.Message, conversationHistory);

            _logger.LogInformation("AI response generated successfully");

            return Ok(new ChatResponse
            {
                Message = aiResponse,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing chat message. Message: {Message}, StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
            return StatusCode(500, new 
            { 
                error = "An error occurred processing your message",
                details = ex.Message, // Added for debugging (remove in prod)
                message = "Sorry, I'm experiencing technical difficulties. Please contact our team directly at (350) 5045930."
            });
        }

    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", service = "chat", timestamp = DateTime.UtcNow });
    }

    [HttpPost("echo")]
    public IActionResult Echo([FromBody] ChatRequest request)
    {
        return Ok(new { message = "Echo: " + request.Message });
    }
}

// DTOs
public class ChatRequest
{
    [System.Text.Json.Serialization.JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    
    [System.Text.Json.Serialization.JsonPropertyName("conversationHistory")]
    public List<MessageDto>? ConversationHistory { get; set; }
}

public class MessageDto
{
    [System.Text.Json.Serialization.JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;
    
    [System.Text.Json.Serialization.JsonPropertyName("isUser")]
    public bool IsUser { get; set; }
}

public class ChatResponse
{
    [System.Text.Json.Serialization.JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    
    [System.Text.Json.Serialization.JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }
}
