using Firmness.Application.Interfaces;
using Firmness.Application.AI.Mappers;
using System.Text.Json;

namespace Firmness.Application.Services.AI;

// Executes the functions requested by Gemini AI
// Converts database data into simple formats for AI
public class GeminiFunctionExecutor
{
    private readonly IVehicleService _vehicleService;
    private readonly IProductService _productService;

    public GeminiFunctionExecutor(IVehicleService vehicleService, IProductService productService)
    {
        _vehicleService = vehicleService ?? throw new ArgumentNullException(nameof(vehicleService));
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
    }

    public async Task<string> ExecuteAsync(string functionName, JsonElement arguments)
    {
        try
        {
            return functionName switch
            {
                "get_vehicle_by_id" => await GetVehicleByIdAsync(arguments),
                "get_available_vehicles" => await GetAvailableVehiclesAsync(arguments),
                "get_product_by_id" => await GetProductByIdAsync(arguments),
                "get_available_products" => await GetAvailableProductsAsync(arguments),
                "check_vehicle_availability" => await CheckVehicleAvailabilityAsync(arguments),
                _ => JsonSerializer.Serialize(new { error = $"Funtion '{functionName}' no implemmnt" })
            };
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { error = $"Error executing function: {ex.Message}" });
        }
    }

    private async Task<string> GetVehicleByIdAsync(JsonElement arguments)
    {
        if (!arguments.TryGetProperty("id", out var idProp) ||
            !Guid.TryParse(idProp.GetString(), out var id))
        {
            return JsonSerializer.Serialize(new { error = "Invalid vehicle ID" });
        }

        var result = await _vehicleService.GetVehicleByIdAsync(id);
        if (!result.IsSuccess || result.Value == null)
        {
            return JsonSerializer.Serialize(new { error = "Vehicle not found" });
        }

        var summary = VehicleMapper.ToSummary(result.Value);
        return JsonSerializer.Serialize(summary);
    }

    private async Task<string> GetAvailableVehiclesAsync(JsonElement arguments)
    {
        // Parámetro opcional: max_results
        var maxResults = arguments.TryGetProperty("max_results", out var max)
            ? Math.Min(max.GetInt32(), 50)
            : 10;

        var result = await _vehicleService.GetAvailableVehiclesAsync();
        if (!result.IsSuccess || result.Value == null)
        {
            return JsonSerializer.Serialize(new { vehicles = Array.Empty<object>(), count = 0 });
        }

        var list = result.Value
            .Take(maxResults)
            .Select(VehicleMapper.ToSummary)
            .ToList();

        return JsonSerializer.Serialize(new { vehicles = list, count = list.Count });
    }

    private async Task<string> GetProductByIdAsync(JsonElement arguments)
    {
        if (!arguments.TryGetProperty("id", out var idProp) ||
            !Guid.TryParse(idProp.GetString(), out var id))
        {
            return JsonSerializer.Serialize(new { error = "Invalid product ID" });
        }

        var result = await _productService.GetByIdAsync(id);
        if (!result.IsSuccess || result.Value == null)
        {
            return JsonSerializer.Serialize(new { error = "Product not found" });
        }

        var summary = ProductMapper.ToSummary(result.Value);
        return JsonSerializer.Serialize(summary);
    }

    private async Task<string> GetAvailableProductsAsync(JsonElement arguments)
    {
        // Parámetro opcional: max_results
        var maxResults = arguments.TryGetProperty("max_results", out var max)
            ? Math.Min(max.GetInt32(), 50)
            : 10;

        var result = await _productService.GetPagedWithCategoryAsync(1, maxResults);
        if (!result.IsSuccess || result.Value == null)
        {
            return JsonSerializer.Serialize(new { products = Array.Empty<object>(), count = 0 });
        }

        var list = result.Value.Items
            .Where(p => p.IsActive)
            .Select(ProductMapper.ToSummary)
            .ToList();

        return JsonSerializer.Serialize(new { products = list, count = list.Count });
    }

    private async Task<string> CheckVehicleAvailabilityAsync(JsonElement arguments)
    {
        if (!arguments.TryGetProperty("vehicle_id", out var vIdProp) ||
            !Guid.TryParse(vIdProp.GetString(), out var vehicleId))
        {
            return JsonSerializer.Serialize(new { error = "Invalid vehicle ID" });
        }

        if (!arguments.TryGetProperty("start_date", out var sDateProp) ||
            !DateTime.TryParse(sDateProp.GetString(), out var startDate))
        {
            return JsonSerializer.Serialize(new { error = "Invalid start date" });
        }

        if (!arguments.TryGetProperty("end_date", out var eDateProp) ||
            !DateTime.TryParse(eDateProp.GetString(), out var endDate))
        {
            return JsonSerializer.Serialize(new { error = "Invalid end datea" });
        }

        var result = await _vehicleService.CheckAvailabilityAsync(vehicleId, startDate, endDate);
        if (!result.IsSuccess || result.Value == null)
        {
            return JsonSerializer.Serialize(new { available = false, error = result.ErrorMessage });
        }

        return JsonSerializer.Serialize(new
        {
            available = result.Value.IsAvailable,
            vehicle_id = vehicleId,
            start_date = startDate.ToString("yyyy-MM-dd"),
            end_date = endDate.ToString("yyyy-MM-dd"),
            conflicts = result.Value.ConflictingRentals?.Count ?? 0
        });
    }
}
