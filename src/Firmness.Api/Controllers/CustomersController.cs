using AutoMapper;
using Firmness.Application.Interfaces;
using Firmness.Application.DTOs;
using Firmness.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Firmness.Infrastructure.Identity;

namespace Firmness.Api.Controllers;


// RESTful controller for customer management
// Implements complete CRUD operations using DTOs and AutoMapper

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly IMapper _mapper;
    private readonly ILogger<CustomersController> _logger;
    private readonly UserManager<ApplicationUser> _userManager;

    public CustomersController(
        ICustomerService customerService,
        IMapper mapper,
        ILogger<CustomersController> logger,
        UserManager<ApplicationUser> userManager)
    {
        _customerService = customerService;
        _mapper = mapper;
        _logger = logger;
        _userManager = userManager;
    }


    // GET: api/customers
    // It obtains all customers (information limited for security reasons)

    // GET: api/customers/me
    // Obtains the current authenticated customer's information
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerDto>> GetMe()
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User ID not found in token" });

            var customer = await _customerService.GetByIdentityUserIdAsync(userId);
            if (customer == null)
                return NotFound(new { message = "Customer profile not found for this user" });

            var customerDto = _mapper.Map<CustomerDto>(customer);
            return Ok(new { isSuccess = true, data = customerDto });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving current customer profile");
            return StatusCode(500, new { message = "Internal Server Error" });
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CustomerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAll()
    {
        try
        {
            var customers = await _customerService.GetAllAsync();
            // Use CustomerDto which does not expose Document, Address, IdentityUserId
            var customerDtos = _mapper.Map<List<CustomerDto>>(customers);
            return Ok(new { isSuccess = true, data = customerDtos });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obtaining clients");
            return StatusCode(500, new { message = "Internal Server Error" });
        }
    }


    // GET: api/customers/{id}
    // Obtains a client by ID (limited information)

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerDto>> GetById(Guid id)
    {
        try
        {
            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null)
                return NotFound(new { message = $"Client with ID {id} not found" });

            var customerDto = _mapper.Map<CustomerDto>(customer);
            return Ok(new { isSuccess = true, data = customerDto });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obtaining client {CustomerId}", id);
            return StatusCode(500, new { message = "Internal Server Error" });
        }
    }


    // GET: api/customers/{id}/details
    // Obtains complete customer information (Administrators only)

    [HttpGet("{id:guid}/details")]
    [Authorize(Policy = "RequireAdminRole")]
    [ProducesResponseType(typeof(CustomerDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerDetailDto>> GetDetailsById(Guid id)
    {
        try
        {
            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null)
                return NotFound(new { message = $"Client with ID {{id}} not found" });
            
            var customerDetailDto = _mapper.Map<CustomerDetailDto>(customer);
            return Ok(new { isSuccess = true, data = customerDetailDto });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customer details {CustomerId}", id);
            return StatusCode(500, new { message = "Internal Server Error" });
        }
    }

  
    // POST: api/customers
    // Create a new client

    [HttpPost]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CustomerDto>> Create([FromBody] CreateCustomerDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Map DTO to entity
            var customer = _mapper.Map<Customer>(createDto);
            await _customerService.AddAsync(customer);

            // Return CustomerDto that does not expose sensitive information
            var customerDto = _mapper.Map<CustomerDto>(customer);
            _logger.LogInformation("Client successfully created: {CustomerId} - {CustomerName}", customer.Id, customer.FullName);

            return CreatedAtAction(nameof(GetById), new { id = customer.Id }, new { isSuccess = true, data = customerDto });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating client");
            return StatusCode(500, new { message = "Internal Server Error" });
        }
    }


    // PUT: api/customers/{id}
    // Update an existing client (authentication required)

    [HttpPut("{id:guid}")]
    [Authorize] // Client must be authenticated
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerDto>> Update(Guid id, [FromBody] UpdateCustomerDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null)
                return NotFound(new { message = $"Client with ID {id} not found" });

            _logger.LogInformation("Updating customer {Id}. Current IsActive: {CurrentIsActive}. DTO IsActive: {DtoIsActive}", 
                    id, customer.IsActive, updateDto.IsActive.HasValue ? updateDto.IsActive.Value.ToString() : "NULL");

            // Security: Prevent non-admins from changing IsActive status
            // If the user is NOT an admin, we force IsActive to null so AutoMapper ignores it (preserving current state)
            if (!User.IsInRole("Admin"))
            {
                updateDto.IsActive = null;
            }

            // Map updateDto to customer (non-null properties only)
            _mapper.Map(updateDto, customer);
            
            _logger.LogInformation("After mapping. New IsActive: {NewIsActive}", customer.IsActive);

            await _customerService.UpdateAsync(customer);

            var customerDto = _mapper.Map<CustomerDto>(customer);
            _logger.LogInformation("Client successfully updated: {CustomerId}", id);

            return Ok(new { isSuccess = true, data = customerDto, message = "Client successfully updated" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating cliente {CustomerId}", id);
            return StatusCode(500, new { message = "Internal Server Error" });
        }
    }
    
    // DELETE: api/customers/{id}
    // Delete a client (Administrators only)
    // Also deletes the associated AuthUser to prevent "email already in use" errors
  
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "RequireAdminRole")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null)
                return NotFound(new { message = $"Client with ID {id} not found"});

            // 1. Delete Customer first
            await _customerService.DeleteAsync(id);
            _logger.LogInformation("Customer deleted: {CustomerId}", id);

            // 2. Delete associated AuthUser (if exists)
            if (!string.IsNullOrEmpty(customer.IdentityUserId))
            {
                var user = await _userManager.FindByIdAsync(customer.IdentityUserId);
                if (user != null)
                {
                    var deleteResult = await _userManager.DeleteAsync(user);
                    if (deleteResult.Succeeded)
                    {
                        _logger.LogInformation(
                            "AuthUser deleted: {UserId} ({Email})", 
                            customer.IdentityUserId, 
                            customer.Email);
                    }
                    else
                    {
                        _logger.LogWarning(
                            "Failed to delete AuthUser: {UserId}. Errors: {Errors}",
                            customer.IdentityUserId,
                            string.Join(", ", deleteResult.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    _logger.LogWarning(
                        "AuthUser not found for customer: {CustomerId} (IdentityUserId: {IdentityUserId})",
                        id,
                        customer.IdentityUserId);
                }
            }

            _logger.LogInformation("Client and AuthUser successfully removed: {CustomerId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting client {CustomerId}", id);
            return StatusCode(500, new { message = "Internal Server Error" });
        }
    }
}
