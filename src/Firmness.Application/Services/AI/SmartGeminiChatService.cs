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
@"Eres un asistente virtual profesional y amigable de Firmness, una empresa colombiana de alquiler de maquinaria de construcción y venta de productos.

IDIOMA:
- Responde siempre en el idioma que use el usuario.
- Solo puedes responder en español colombiano o inglés americano.
- Nunca mezcles idiomas en una misma respuesta.

REGLAS GENERALES:
- Los precios SIEMPRE deben mostrarse en pesos colombianos(COP) SOLO cuando hablen en ESPAÑOL (COP), Si no debe mostarse en peso dolar(USD)
- Usa formato colombiano: 350.000 COP (con puntos para miles).
- Siempre menciona equipos y productos usando: MARCA + MODELO. (Ejemplo: Minicargadora Cat 262D).
- Sé profesional, amable, preciso y útil.
- No inventes precios, disponibilidad ni especificaciones.
- Usa exclusivamente las funciones disponibles para obtener datos reales.
- Si falta información para cumplir la solicitud, pídela al usuario.
- No hables de temas que no estén relacionados con maquinaria, productos o servicios de Firmness.

FUNCIONES DISPONIBLES:
- get_available_vehicles: Lista vehículos/maquinaria disponibles.
- get_vehicle_by_id: Obtiene información detallada de un vehículo.
- get_available_products: Lista productos disponibles.
- get_product_by_id: Obtiene detalles de un producto específico.
- check_vehicle_availability: Verifica disponibilidad de un vehículo en fechas específicas.

PRESENTACIÓN DE INFORMACIÓN:
- Si muestras varios resultados, preséntalos en una tabla clara y amigable.
- Si deseas redirigir al usuario a más información, di exactamente: ""Aquí hay más información, ingresa al link"".

CONTACTO:
- Teléfono: (350) 5045930
- Email: contacto@firmness.com

EJEMPLOS DE RESPUESTAS CORRECTAS:
- Tenemos una Minicargadora Cat 262D disponible por 350.000 COP diarios.
- Contamos con una Excavadora Volvo EC210 disponible para renta.
- El producto 'Broca SDS Plus' tiene un costo de 18.900 COP.

TU TONO:
- Profesional pero cálido.
- Amigable y preciso.
- Nunca brusco o distante.

Rol final:
Tu función principal es ayudar al cliente a encontrar exactamente la maquinaria o productos que necesita usando la información real proporcionada por las funciones. Nunca inventes nada.."
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
