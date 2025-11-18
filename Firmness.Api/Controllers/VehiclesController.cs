using AutoMapper;
using Firmness.Application.Interfaces;
using Firmness.Domain.DTOs.Vehicle;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Firmness.Api.Controllers;

// RESTful controller for vehicle management
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VehiclesController : ControllerBase
{
    private readonly IVehicleService _vehicleService;
    private readonly IMapper _mapper;
    private readonly ILogger<VehiclesController> _logger;

    public VehiclesController(
        IVehicleService vehicleService,
        IMapper mapper,
        ILogger<VehiclesController> logger)
    {
        _vehicleService = vehicleService;
        _mapper = mapper;
        _logger = logger;
    }

    // GET: api/vehicles
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<VehicleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var result = await _vehicleService.GetAllVehiclesAsync();
            if (!result.IsSuccess)
                return BadRequest(new { isSuccess = false, message = result.ErrorMessage });

            return Ok(new
            {
                isSuccess = true,
                data = result.Value,
                message = "Vehicles retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving vehicles");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    // GET: api/vehicles/available
    [HttpGet("available")]
    [ProducesResponseType(typeof(IEnumerable<VehicleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAvailable()
    {
        try
        {
            var result = await _vehicleService.GetAvailableVehiclesAsync();
            if (!result.IsSuccess)
                return BadRequest(new { isSuccess = false, message = result.ErrorMessage });

            return Ok(new
            {
                isSuccess = true,
                data = result.Value,
                message = "Available vehicles retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving available vehicles");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    // GET: api/vehicles/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(VehicleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var result = await _vehicleService.GetVehicleByIdAsync(id);
            if (!result.IsSuccess)
                return NotFound(new { isSuccess = false, message = result.ErrorMessage });

            return Ok(new
            {
                isSuccess = true,
                data = result.Value,
                message = "Vehicle retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving vehicle {VehicleId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    // GET: api/vehicles/type/{vehicleType}
    [HttpGet("type/{vehicleType}")]
    [ProducesResponseType(typeof(IEnumerable<VehicleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByType(string vehicleType)
    {
        try
        {
            var result = await _vehicleService.GetVehiclesByTypeAsync(vehicleType);
            if (!result.IsSuccess)
                return BadRequest(new { isSuccess = false, message = result.ErrorMessage });

            return Ok(new
            {
                isSuccess = true,
                data = result.Value,
                message = $"Vehicles of type {vehicleType} retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving vehicles by type {VehicleType}", vehicleType);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    // POST: api/vehicles
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(VehicleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateVehicleDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _vehicleService.CreateVehicleAsync(createDto);
            if (!result.IsSuccess)
                return BadRequest(new { isSuccess = false, message = result.ErrorMessage });

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Value!.Id },
                new
                {
                    isSuccess = true,
                    data = result.Value,
                    message = "Vehicle created successfully"
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating vehicle");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    // PUT: api/vehicles/{id}
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(VehicleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateVehicleDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _vehicleService.UpdateVehicleAsync(id, updateDto);
            if (!result.IsSuccess)
                return NotFound(new { isSuccess = false, message = result.ErrorMessage });

            return Ok(new
            {
                isSuccess = true,
                data = result.Value,
                message = "Vehicle updated successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating vehicle {VehicleId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    // DELETE: api/vehicles/{id}
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _vehicleService.DeleteVehicleAsync(id);
            if (!result.IsSuccess)
                return NotFound(new { isSuccess = false, message = result.ErrorMessage });

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting vehicle {VehicleId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    // PATCH: api/vehicles/{id}/status
    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusDto statusDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _vehicleService.UpdateVehicleStatusAsync(id, statusDto.Status);
            if (!result.IsSuccess)
                return NotFound(new { isSuccess = false, message = result.ErrorMessage });

            return Ok(new
            {
                isSuccess = true,
                message = "Vehicle status updated successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating vehicle status {VehicleId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    // POST: api/vehicles/{id}/check-availability
    [HttpPost("{id:guid}/check-availability")]
    [ProducesResponseType(typeof(VehicleAvailabilityResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckAvailability(Guid id, [FromBody] VehicleAvailabilityDto availabilityDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _vehicleService.CheckAvailabilityAsync(
                id,
                availabilityDto.StartDate,
                availabilityDto.EndDate);

            if (!result.IsSuccess)
                return NotFound(new { isSuccess = false, message = result.ErrorMessage });

            return Ok(new
            {
                isSuccess = true,
                data = result.Value,
                message = "Availability checked successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking availability for vehicle {VehicleId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}

// DTO for updating vehicle status
public class UpdateStatusDto
{
    public string Status { get; set; } = string.Empty;
}

