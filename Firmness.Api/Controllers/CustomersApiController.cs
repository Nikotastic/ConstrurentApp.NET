using AutoMapper;
using Firmness.Domain.DTOs;
using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Firmness.Api.Controllers;


// API Controller for Customers - DTOs
[ApiController]
[Route("api/[controller]")]
public class CustomersApiController : ControllerBase
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CustomersApiController> _logger;

    public CustomersApiController(
        ICustomerRepository customerRepository,
        IMapper mapper,
        ILogger<CustomersApiController> logger)
    {
        _customerRepository = customerRepository;
        _mapper = mapper;
        _logger = logger;
    }


    // Get all customers - Information limit
    // Public access
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAll()
    {
        var customers = await _customerRepository.GetAllAsync();
        // Use CustomerDto that does not expose Document, Address, IdentityUserId
        var customerDtos = _mapper.Map<List<CustomerDto>>(customers);
        return Ok(customerDtos);
    }
    
    // Get Customer by ID - Limit information

    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerDto>> GetById(Guid id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
            return NotFound();

        var customerDto = _mapper.Map<CustomerDto>(customer);
        return Ok(customerDto);
    }
    
    //Get all information - Just Admin
    // Admin access only - Full information
    [HttpGet("{id}/details")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<ActionResult<CustomerDetailDto>> GetDetailsById(Guid id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
            return NotFound();

        // Use CustomerDetailDto that includes Document, Address, IdentityUserId
        var customerDetailDto = _mapper.Map<CustomerDetailDto>(customer);
        return Ok(customerDetailDto);
    }
    
    // Create customer
    [HttpPost]
    public async Task<ActionResult<CustomerDto>> Create(CreateCustomerDto createDto)
    {
        var customer = _mapper.Map<Customer>(createDto);
        await _customerRepository.AddAsync(customer);
        
        // Use CustomerDto that does not expose Document, Address, IdentityUserId
        var customerDto = _mapper.Map<CustomerDto>(customer);
        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customerDto);
    }
    
    // Update customer
    [HttpPut("{id}")]
    [Authorize] // Customer must be authenticated
    public async Task<ActionResult<CustomerDto>> Update(Guid id, UpdateCustomerDto updateDto)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
            return NotFound();

        // Map updateDto to customer
        _mapper.Map(updateDto, customer);
        await _customerRepository.UpdateAsync(customer);
        
        var customerDto = _mapper.Map<CustomerDto>(customer);
        return Ok(customerDto);
    }
    
    // Delete customer - Just Admin
    [HttpDelete("{id}")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
            return NotFound();

        await _customerRepository.DeleteAsync(id);
        return NoContent();
    }
}

