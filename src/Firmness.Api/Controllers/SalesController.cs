using AutoMapper;
using Firmness.Application.Interfaces;
using Firmness.Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Firmness.Api.Controllers;


// RESTful controller for sales management
// Implements complete CRUD operations using DTOs and AutoMapper

[ApiController]
[Route("api/[controller]")]
[Authorize] // Requires authentication by default
public class SalesController : ControllerBase
{
    private readonly ISaleService _saleService;
    private readonly IMapper _mapper;
    private readonly ILogger<SalesController> _logger;

    public SalesController(
        ISaleService saleService,
        IMapper mapper,
        ILogger<SalesController> logger)
    {
        _saleService = saleService;
        _mapper = mapper;
        _logger = logger;
    }

    // GET: api/sales
    //Gets all sales (Administrators Only)

    [HttpGet]
    [Authorize(Policy = "RequireAdminRole")]
    [ProducesResponseType(typeof(IEnumerable<SaleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var sales = await _saleService.GetAllWithDetailsAsync();
            var saleDtos = _mapper.Map<IEnumerable<SaleDto>>(sales);
            return Ok(new { isSuccess = true, data = saleDtos });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obtaining sales");
            return StatusCode(500, new { message = "Internal Server Error" });
        }
    }


    // GET: api/sales/{id}
    // Get a specific sale by your ID

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SaleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var sale = await _saleService.GetByIdWithDetailsAsync(id);
            if (sale == null)
                return NotFound(new { message = $"Sale with ID {{id}} not found" });

            var saleDto = _mapper.Map<SaleDto>(sale);
            return Ok(new { isSuccess = true, data = saleDto });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obtaining sale {SaleId}", id);
            return StatusCode(500, new { message = "Internal Server Errorr" });
        }
    }

    // GET: api/sales/customer/{customerId}
    // It obtains all sales from a specific customer

    [HttpGet("customer/{customerId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<SaleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCustomerId(Guid customerId)
    {
        try
        {
            var sales = await _saleService.GetByCustomerIdAsync(customerId);
            var saleDtos = _mapper.Map<IEnumerable<SaleDto>>(sales);
            return Ok(new { isSuccess = true, data = saleDtos });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obtaining customer sales {CustomerId}", customerId);
            return StatusCode(500, new { message = "Internal Server Error" });
        }
    }

    // POST: api/sales
    // Create a new sale with stock validation

    [HttpPost]
    [ProducesResponseType(typeof(SaleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateSaleDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validate that there are items in the sale
            if (createDto.Items == null || !createDto.Items.Any())
                return BadRequest(new { message = "The sale must contain at least one product" });

            // Create the sale using the service
            var lines = createDto.Items.Select(i => (i.ProductId, i.Quantity));
            var sale = await _saleService.CreateSaleAsync(createDto.CustomerId, lines);

            // Apply additional information from the DTO
            sale.PaymentMethod = createDto.PaymentMethod;
            sale.Discount = createDto.Discount;
            sale.Tax = createDto.Tax;
            sale.Notes = createDto.Notes;

            // Recalculate the total with discount and taxes
            sale.Subtotal = sale.Items.Sum(i => i.LineaTotal);
            sale.TotalAmount = sale.Subtotal - sale.Discount + sale.Tax;

            await _saleService.UpdateAsync(sale);

            // Get the full sale with details
            var saleWithDetails = await _saleService.GetByIdWithDetailsAsync(sale.Id);
            var saleDto = _mapper.Map<SaleDto>(saleWithDetails);

            _logger.LogInformation("Sale successfully created: {SaleId} - Total: {Total}", sale.Id, sale.TotalAmount);

            return CreatedAtAction(nameof(GetById), new { id = sale.Id }, new { isSuccess = true, data = saleDto });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Validation error when creating sale");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating sale");
            return StatusCode(500, new { message = "Internal Server Error" });
        }
    }

    // PUT: api/sales/{id}
    // Update an existing sale (Administrators only)
   
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "RequireAdminRole")]
    [ProducesResponseType(typeof(SaleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSaleDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var sale = await _saleService.GetByIdWithDetailsAsync(id);
            if (sale == null)
                return NotFound(new { message = $"Sale with ID {id} not found" });
            
            _mapper.Map(updateDto, sale);

            // Recalculate totals if there are changes
            if (updateDto.Discount.HasValue || updateDto.Tax.HasValue)
            {
                sale.Subtotal = sale.Items.Sum(i => i.LineaTotal);
                sale.TotalAmount = sale.Subtotal - sale.Discount + sale.Tax;
            }

            await _saleService.UpdateAsync(sale);

            var saleDto = _mapper.Map<SaleDto>(sale);
            _logger.LogInformation("Sale successfully updated: {SaleId}", id);

            return Ok(new { isSuccess = true, data = saleDto, message = "Sale successfully updated" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating sale {SaleId}", id);
            return StatusCode(500, new { message = "Internal Server Error" });
        }
    }

    // DELETE: api/sales/{id}
    // Delete a sale (Administrators only)
  
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "RequireAdminRole")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var sale = await _saleService.GetByIdAsync(id);
            if (sale == null)
                return NotFound(new { message = $"Sale with ID {id} not found" });

            await _saleService.DeleteAsync(id);
            _logger.LogInformation("Sale successfully removed: {SaleId}", id);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting sale {SaleId}", id);
            return StatusCode(500, new { message = "Internal Server Error"});
        }
    }

    // GET: api/sales/count
    // Get the total sales in the system (Administrators only)
   
    [HttpGet("count")]
    [Authorize(Policy = "RequireAdminRole")]
    [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
    public async Task<IActionResult> Count()
    {
        try
        {
            var result = await _saleService.CountAsync();
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(new { isSuccess = true, data = result.Value, message = "Count successfully completed" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in counting sales");
            return StatusCode(500, new { message = "Internal Server Error" });
        }
    }
}
