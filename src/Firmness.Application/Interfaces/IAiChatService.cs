namespace Firmness.Application.Interfaces;

public interface IAiChatService
{
    // Sends a message to the AI and gets a response
    /// <param name="userMessage">The user's message</param>
    /// <param name="conversationHistory">Optional conversation history for context</param>
    /// <returns>AI response message</returns>
    Task<string> GetAiResponseAsync(string userMessage, List<ChatMessage>? conversationHistory = null);
}

public class ChatMessage
{
    public string Role { get; set; } = string.Empty; 
    public string Content { get; set; } = string.Empty;
}
