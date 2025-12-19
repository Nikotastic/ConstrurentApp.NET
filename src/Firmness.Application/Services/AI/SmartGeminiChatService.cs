using Firmness.Application.Interfaces;
using Firmness.Application.Services.AI.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace Firmness.Application.Services.AI;

// Intelligent chat service using Gemini Function Calling
// Enables AI to decide when to query the database for real-time data
public class SmartGeminiChatService : IAiChatService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model;
    private readonly GeminiFunctionExecutor _functionExecutor;
    private readonly ILogger<SmartGeminiChatService> _logger;

    public SmartGeminiChatService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        IVehicleService vehicleService,
        IProductService productService,
        ILogger<SmartGeminiChatService> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger; 
        
        // Try multiple sources for API Key
        // Note: In .NET configuration, "Gemini:ApiKey" from appsettings can also be set as "Gemini__ApiKey" env var
        _apiKey = configuration["Gemini:ApiKey"]           // From appsettings.json
                  ?? configuration["Gemini__ApiKey"]       // From environment variable (Docker style)
                  ?? Environment.GetEnvironmentVariable("GEMINI_API_KEY")  // Direct env var
                  ?? throw new InvalidOperationException("Gemini API Key not configured");
        
        _logger.LogInformation("Gemini API Key loaded successfully (length: {Length})", _apiKey.Length);

        var modelName = configuration["Gemini:Model"] ?? "gemini-2.0-flash";
        _model = modelName.StartsWith("models/") ? modelName.Substring(7) : modelName;
        
        _functionExecutor = new GeminiFunctionExecutor(vehicleService, productService);
    }

    public async Task<string> GetAiResponseAsync(string userMessage, List<ChatMessage>? conversationHistory = null)
    {
        try
        {
            _logger.LogInformation("Processing chat message: {Message}", userMessage);

            // Build conversation with function calling support
            var messages = BuildConversationMessages(userMessage, conversationHistory);

            // Get available tools
            var tools = ToolsConfig.GetTools();

            // Initial request to Gemini
            var response = await CallGeminiWithFunctions(messages, tools);

            // Handle function calls if Gemini requests them
            var maxIterations = 5; // Prevent infinite loops
            var iteration = 0;

            while (response.NeedsFunctionCall && iteration < maxIterations)
            {
                iteration++;
                _logger.LogInformation("Gemini requested function call #{Iteration}: {FunctionName}", 
                    iteration, response.FunctionName);

                // Execute the requested function
                var functionResult = await _functionExecutor.ExecuteAsync(
                    response.FunctionName!, 
                    response.FunctionArgs!.Value);

                // Send function result back to Gemini
                response = await CallGeminiWithFunctionResult(
                    messages, 
                    tools, 
                    response.FunctionName!, 
                    functionResult);
            }

            if (iteration >= maxIterations)
            {
                _logger.LogWarning("Max function call iterations reached");
            }

            return response.TextResponse ?? "I'm sorry, I couldn't generate a response..";
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error calling Gemini API");
            return "Sorry, I'm experiencing technical difficulties. Please contact our team at (350) 5045930.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in AI service");
            return "An unexpected error occurred. Please try again later..";
        }
    }

    private List<object> BuildConversationMessages(string userMessage, List<ChatMessage>? history)
    {
        var messages = new List<object>();

        // SYSTEM prompt (Gemini 2.0 format)
        messages.Add(new
        {
            role = "model",
            parts = new object[]
            {
                new
                {
                    text =
@"You are a professional and friendly virtual assistant for Firmness, a Colombian company specialized in construction machinery rental and product sales.

LANGUAGE RULES:

Always respond in the same language the user uses.

You may only answer in American English or Colombian Spanish.

Never mix both languages in the same response.

GENERAL RULES:

Prices must ALWAYS be shown in Colombian pesos (COP) ONLY when speaking in Spanish.

If the user speaks in English, prices must be shown in US dollars (USD).

Use Colombian currency formatting, for example: 350.000 COP (dots for thousands).

Always mention equipment and products using: BRAND + MODEL

Example: Skid Steer Loader Cat 262D

Be professional, friendly, clear, and helpful.

Do not invent prices, availability, or specifications.

Use only the available functions to retrieve real information.

If you don’t have enough information to answer, ask the user for it.

Do not talk about topics unrelated to Firmness, machinery, products, or construction services.

AVAILABLE FUNCTIONS:

get_available_vehicles: Lists available vehicles/machinery.

get_vehicle_by_id: Gets detailed information about a vehicle.

get_available_products: Lists available products.

get_product_by_id: Gets details of a specific product.

check_vehicle_availability: Checks availability for specific dates.

INFORMATION PRESENTATION:

Do not show many results at once, and if you show them, make them visually clear and attractive.

If you want to direct the user to more information, say exactly: 'Here is more information, enter the link'

CONTACT INFORMATION:

Phone: (350) 5045930

Email: contacto@firmness.com

EXAMPLES OF CORRECT ANSWERS:

We have a Cat 262D Skid Steer Loader available for 350.000 COP per day.

We offer a Volvo EC210 Excavator available for rental.

The product “SDS Plus Drill Bit” costs 18.900 COP.

TONE GUIDELINES:

Professional but warm.

Friendly, clear, and precise.

Never harsh or distant.

FINAL ROLE:
Your main function is to help customers find the exact machinery or products they need using real information retrieved from functions.
Never invent details, and do not talk about topics unrelated to construction"
                }
            }
        });

        // HISTORY
        if (history != null)
        {
            foreach (var msg in history.TakeLast(5))
            {
                messages.Add(new
                {
                    role = msg.Role == "user" ? "user" : "model",
                    parts = new[] { new { text = msg.Content } }
                });
            }
        }

        // USER message
        messages.Add(new
        {
            role = "user",
            parts = new[] { new { text = userMessage } }
        });

        return messages;
    }

    private async Task<GeminiResponse> CallGeminiWithFunctions(List<object> messages, object tools)
    {
        var requestBody = new
        {
            contents = messages,
            tools = tools,
            generationConfig = new
            {
                temperature = 0.7,
                maxOutputTokens = 1000,
                topP = 0.95,
                topK = 40
            }
        };

        return await SendGeminiRequest(requestBody);
    }

    private async Task<GeminiResponse> CallGeminiWithFunctionResult(
        List<object> messages,
        object tools,
        string functionName,
        string functionResult)
    {
        // Add function response to messages
        messages.Add(new
        {
            role = "function",
            parts = new[]
            {
                new
                {
                    functionResponse = new
                    {
                        name = functionName,
                        response = new { content = functionResult }
                    }
                }
            }
        });

        var requestBody = new
        {
            contents = messages,
            tools = tools,
            generationConfig = new
            {
                temperature = 0.7,
                maxOutputTokens = 1000,
                topP = 0.95,
                topK = 40
            }
        };

        return await SendGeminiRequest(requestBody);
    }

    private async Task<GeminiResponse> SendGeminiRequest(object requestBody)
    {
        // Usar query parameter para la API key (método más confiable)
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";
        
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
        
        var jsonBody = JsonSerializer.Serialize(requestBody, jsonOptions);

        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
        };

        _logger.LogInformation("Sending request to Gemini API (using query parameter for auth)");
        _logger.LogDebug("Request body: {RequestBody}", jsonBody.Length > 500 ? jsonBody.Substring(0, 500) + "..." : jsonBody);

        var response = await _httpClient.SendAsync(request);

        _logger.LogInformation("Gemini API response status: {StatusCode}", response.StatusCode);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("Gemini API error response: {ErrorContent}", errorContent);
            response.EnsureSuccessStatusCode();
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        _logger.LogDebug("Gemini response received: {Length} characters", responseContent.Length);

        var geminiResponse = JsonSerializer.Deserialize<GeminiApiResponse>(responseContent);

        if (geminiResponse?.Candidates == null || !geminiResponse.Candidates.Any())
        {
            _logger.LogWarning("No candidates in Gemini response");
            return new GeminiResponse { TextResponse = null, NeedsFunctionCall = false };
        }

        var candidate = geminiResponse.Candidates.First();
        var part = candidate.Content?.Parts?.FirstOrDefault();

        // Detect if Gemini requests to call a function
        if (part?.FunctionCall != null)
        {
            _logger.LogInformation("Gemini requested function: {FunctionName}", part.FunctionCall.Name);
            return new GeminiResponse
            {
                NeedsFunctionCall = true,
                FunctionName = part.FunctionCall.Name,
                FunctionArgs = part.FunctionCall.Args
            };
        }

        // Normal text response
        var text = part?.Text;
        _logger.LogInformation("Gemini returned text response");
        return new GeminiResponse
        {
            TextResponse = text,
            NeedsFunctionCall = false
        };
    }
}
