﻿using AutoMapper;
 using Firmness.Application.Interfaces;
 using Firmness.Domain.DTOs;
using Firmness.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Firmness.Api.Controllers;


// RESTful controller for customer management
// Implements complete CRUD operations using DTOs and AutoMapper

[ApiController]
[Route("api/[controller]")]
public class CustomersApiController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly IMapper _mapper;
    private readonly ILogger<CustomersApiController> _logger;

    public CustomersApiController(
        ICustomerService customerService,
        IMapper mapper,
        ILogger<CustomersApiController> logger)
    {
        _customerService = customerService;
        _mapper = mapper;
        _logger = logger;
    }


    // GET: api/customers
    // It obtains all customers (information limited for security reasons)

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

            // Map updateDto to customer (non-null properties only)
            _mapper.Map(updateDto, customer);
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

            await _customerService.DeleteAsync(id);
            _logger.LogInformation("Client successfully removed: {CustomerId}", id);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting client {CustomerId}", id);
            return StatusCode(500, new { message = "Internal Server Error" });
        }
    }
}

