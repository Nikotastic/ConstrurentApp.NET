using AutoMapper;
using Firmness.Application.Interfaces;
using Firmness.Application.DTOs.Vehicle;
using Firmness.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Firmness.Api.Controllers;

// RESTful controller for vehicle rental management
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VehicleRentalsController : ControllerBase
{
    private readonly IVehicleRentalService _rentalService;
    private readonly ICustomerService _customerService;
    private readonly IMapper _mapper;
    private readonly ILogger<VehicleRentalsController> _logger;

    public VehicleRentalsController(
        IVehicleRentalService rentalService,
        ICustomerService customerService,
        IMapper mapper,
        ILogger<VehicleRentalsController> logger)
    {
        _rentalService = rentalService;
        _customerService = customerService;
        _mapper = mapper;
        _logger = logger;
    }

    // GET: api/vehiclerentals
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<VehicleRentalDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] bool includeDetails = false)
    {
        try
        {
            var result = includeDetails
                ? await _rentalService.GetAllRentalsWithDetailsAsync()
                : await _rentalService.GetAllRentalsAsync();

            if (!result.IsSuccess)
                return BadRequest(new { isSuccess = false, message = result.ErrorMessage });

            return Ok(new
            {
                isSuccess = true,
                data = result.Value,
                message = "Rentals retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving rentals");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    // GET: api/vehiclerentals/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(VehicleRentalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, [FromQuery] bool includeDetails = true)
    {
        try
        {
            var result = includeDetails
                ? await _rentalService.GetRentalByIdWithDetailsAsync(id)
                : await _rentalService.GetRentalByIdAsync(id);

            if (!result.IsSuccess)
                return NotFound(new { isSuccess = false, message = result.ErrorMessage });

            return Ok(new
            {
                isSuccess = true,
                data = result.Value,
                message = "Rental retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving rental {RentalId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    // GET: api/vehiclerentals/customer/{customerId}
    [HttpGet("customer/{customerId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<VehicleRentalDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCustomer(Guid customerId)
    {
        try
        {
            // Security check: Clients can only view their own rentals
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole("Admin");
            
            if (!isAdmin)
            {
                // Get the customer associated with the current user
                var currentCustomer = await _customerService.GetByIdentityUserIdAsync(currentUserId!);
                
                if (currentCustomer == null || currentCustomer.Id != customerId)
                {
                    _logger.LogWarning("User {UserId} attempted to access rentals for customer {CustomerId}", currentUserId, customerId);
                    return Forbid(); // 403 Forbidden
                }
            }
            
            var result = await _rentalService.GetRentalsByCustomerIdAsync(customerId);
            if (!result.IsSuccess)
                return BadRequest(new { isSuccess = false, message = result.ErrorMessage });

            return Ok(new
            {
                isSuccess = true,
                data = result.Value,
                message = "Customer rentals retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving rentals for customer {CustomerId}", customerId);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    // GET: api/vehiclerentals/vehicle/{vehicleId}
    [HttpGet("vehicle/{vehicleId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<VehicleRentalDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByVehicle(Guid vehicleId)
    {
        try
        {
            var result = await _rentalService.GetRentalsByVehicleIdAsync(vehicleId);
            if (!result.IsSuccess)
                return BadRequest(new { isSuccess = false, message = result.ErrorMessage });

            return Ok(new
            {
                isSuccess = true,
                data = result.Value,
                message = "Vehicle rentals retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving rentals for vehicle {VehicleId}", vehicleId);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    // GET: api/vehiclerentals/status/{status}
    [HttpGet("status/{status}")]
    [ProducesResponseType(typeof(IEnumerable<VehicleRentalDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByStatus(string status)
    {
        try
        {
            if (!Enum.TryParse<RentalStatus>(status, true, out var rentalStatus))
                return BadRequest(new { isSuccess = false, message = "Invalid rental status" });

            var result = await _rentalService.GetRentalsByStatusAsync(rentalStatus);
            if (!result.IsSuccess)
                return BadRequest(new { isSuccess = false, message = result.ErrorMessage });

            return Ok(new
            {
                isSuccess = true,
                data = result.Value,
                message = $"Rentals with status {status} retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving rentals by status {Status}", status);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    // POST: api/vehiclerentals
    [HttpPost]
    [Authorize(Roles = "Admin,Client")]
    [ProducesResponseType(typeof(VehicleRentalDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateVehicleRentalDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _rentalService.CreateRentalAsync(createDto);
            if (!result.IsSuccess)
                return BadRequest(new { isSuccess = false, message = result.ErrorMessage });

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Value!.Id },
                new
                {
                    isSuccess = true,
                    data = result.Value,
                    message = "Rental created successfully"
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating rental");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    // PUT: api/vehiclerentals/{id}
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(VehicleRentalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateVehicleRentalDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _rentalService.UpdateRentalAsync(id, updateDto);
            if (!result.IsSuccess)
                return NotFound(new { isSuccess = false, message = result.ErrorMessage });

            return Ok(new
            {
                isSuccess = true,
                data = result.Value,
                message = "Rental updated successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating rental {RentalId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    // DELETE: api/vehiclerentals/{id}
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _rentalService.DeleteRentalAsync(id);
            if (!result.IsSuccess)
                return NotFound(new { isSuccess = false, message = result.ErrorMessage });

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting rental {RentalId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    // POST: api/vehiclerentals/{id}/complete
    [HttpPost("{id:guid}/complete")]
    [Authorize(Roles = "Admin,Client")]
    [ProducesResponseType(typeof(VehicleRentalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Complete(Guid id, [FromBody] CompleteVehicleRentalDto completeDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _rentalService.CompleteRentalAsync(id, completeDto);
            if (!result.IsSuccess)
                return NotFound(new { isSuccess = false, message = result.ErrorMessage });

            return Ok(new
            {
                isSuccess = true,
                data = result.Value,
                message = "Rental completed successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing rental {RentalId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    // POST: api/vehiclerentals/{id}/cancel
    [HttpPost("{id:guid}/cancel")]
    [Authorize(Roles = "Admin,Client")]
    [ProducesResponseType(typeof(VehicleRentalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelRentalDto cancelDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Security check: Clients can only cancel their own rentals
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole("Admin");
            
            if (!isAdmin)
            {
                // Get the rental to verify ownership
                var rentalResult = await _rentalService.GetRentalByIdAsync(id);
                if (!rentalResult.IsSuccess)
                    return NotFound(new { isSuccess = false, message = "Rental not found" });
                
                // Get the customer associated with the current user
                var currentCustomer = await _customerService.GetByIdentityUserIdAsync(currentUserId!);
                
                if (currentCustomer == null || rentalResult.Value!.CustomerId != currentCustomer.Id)
                {
                    _logger.LogWarning("User {UserId} attempted to cancel rental {RentalId} that doesn't belong to them", currentUserId, id);
                    return Forbid(); // 403 Forbidden
                }
            }

            var result = await _rentalService.CancelRentalAsync(id, cancelDto.CancellationReason);
            if (!result.IsSuccess)
                return BadRequest(new { isSuccess = false, message = result.ErrorMessage });

            return Ok(new
            {
                isSuccess = true,
                data = result.Value,
                message = "Rental cancelled successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling rental {RentalId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    // POST: api/vehiclerentals/{id}/payment
    [HttpPost("{id:guid}/payment")]
    [Authorize(Roles = "Admin,Client")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ProcessPayment(Guid id, [FromBody] PaymentDto paymentDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _rentalService.ProcessPaymentAsync(id, paymentDto.Amount);
            if (!result.IsSuccess)
                return NotFound(new { isSuccess = false, message = result.ErrorMessage });

            return Ok(new
            {
                isSuccess = true,
                message = "Payment processed successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment for rental {RentalId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    // POST: api/vehiclerentals/{id}/return-deposit
    [HttpPost("{id:guid}/return-deposit")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReturnDeposit(Guid id)
    {
        try
        {
            var result = await _rentalService.ReturnDepositAsync(id);
            if (!result.IsSuccess)
                return NotFound(new { isSuccess = false, message = result.ErrorMessage });

            return Ok(new
            {
                isSuccess = true,
                message = "Deposit returned successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error returning deposit for rental {RentalId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}

// DTO for cancelling a rental
public class CancelRentalDto
{
    public string CancellationReason { get; set; } = string.Empty;
}

// DTO for processing payment
public class PaymentDto
{
    public decimal Amount { get; set; }
}

