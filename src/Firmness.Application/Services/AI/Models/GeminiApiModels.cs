using System.Text.Json.Serialization;

namespace Firmness.Application.Services.AI.Models;

// DTOs for Gemini API responses

public class GeminiApiResponse
{
    [JsonPropertyName("candidates")]
    public List<Candidate>? Candidates { get; set; }
}

public class Candidate
{
    [JsonPropertyName("content")]
    public Content? Content { get; set; }
}

public class Content
{
    [JsonPropertyName("parts")]
    public List<Part>? Parts { get; set; }
}

public class Part
{
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    [JsonPropertyName("functionCall")]
    public FunctionCall? FunctionCall { get; set; }
}

public class FunctionCall
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("args")]
    public System.Text.Json.JsonElement Args { get; set; }
}
