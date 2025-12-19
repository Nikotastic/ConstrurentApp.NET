using System.Text.Json;

namespace Firmness.Application.Services.AI.Models;

// Wrapper for Gemini response with function calling support
public class GeminiResponse
{
    public string? TextResponse { get; set; }
    public bool NeedsFunctionCall { get; set; }
    public string? FunctionName { get; set; }
    public JsonElement? FunctionArgs { get; set; }
}
