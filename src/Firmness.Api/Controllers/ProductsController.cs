using AutoMapper;
using Firmness.Application.Interfaces;
using Firmness.Application.DTOs;
using Firmness.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Firmness.Api.Controllers;


// RESTful controller for product management

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService service, IMapper mapper, ILogger<ProductsController> logger)
    {
        _service = service;
        _mapper = mapper;
        _logger = logger;
    }


    // GET: api/products
    [HttpGet]
    [AllowAnonymous] // This endpoint is public (no token required)
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        try
        {
            // Use GetPagedWithCategoryAsync to include category information
            var result = await _service.GetPagedWithCategoryAsync(page, pageSize);
            if (!result.IsSuccess)
                return BadRequest(result);

            // Mapear entidades a DTOs
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(result.Value?.Items);
            return Ok(new
            {
                isSuccess = result.IsSuccess,
                data = new
                {
                    items = productDtos,
                    totalCount = result.Value?.TotalItems,
                    page = result.Value?.Page,
                    pageSize = result.Value?.PageSize
                },
                message = "Productos obtenidos exitosamente"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener productos");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }


    // GET: api/products/search?q=name

    [HttpGet("search")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        try
        {
            var result = await _service.SearchAsync(q, page, pageSize);
            if (!result.IsSuccess)
                return BadRequest(result);

            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(result.Value?.Items);
            return Ok(new
            {
                isSuccess = result.IsSuccess,
                data = new
                {
                    items = productDtos,
                    totalCount = result.Value?.TotalItems,
                    page = result.Value?.Page,
                    pageSize = result.Value?.PageSize
                },
                message = "Búsqueda completada exitosamente"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar productos");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }


    // GET: api/products/{id}

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var result = await _service.GetByIdAsync(id);
            if (!result.IsSuccess || result.Value == null)
                return NotFound(new { message = $"Producto con ID {id} no encontrado" });

            var productDto = _mapper.Map<ProductDto>(result.Value);
            return Ok(new { isSuccess = true, data = productDto });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener producto {ProductId}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

 
    // POST: api/products
    //Create a new product (Administrators only)
    [HttpPost]
    [Authorize(Policy = "RequireAdminRole")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProductDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Map DTO to entity
            var product = _mapper.Map<Product>(createDto);
            
            var result = await _service.AddAsync(product);
            if (!result.IsSuccess)
                return BadRequest(result);

            var productDto = _mapper.Map<ProductDto>(product);
            _logger.LogInformation("Product successfully created: {ProductId} - {ProductName}", product.Id, product.Name);
            
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, new { isSuccess = true, data = productDto });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating producto");
            return StatusCode(500, new { message = "Internal Server Error" });
        }
    }


    // PUT: api/products/{id}
    // Update an existing product (Administrators only)

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "RequireAdminRole")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Obtener producto existente
            var existingResult = await _service.GetByIdAsync(id);
            if (!existingResult.IsSuccess || existingResult.Value == null)
                return NotFound(new { message = $"Producto con ID {id} no encontrado" });

            // Mapear cambios del DTO a la entidad existente
            _mapper.Map(updateDto, existingResult.Value);
            
            var result = await _service.UpdateAsync(existingResult.Value);
            if (!result.IsSuccess)
                return BadRequest(result);

            var productDto = _mapper.Map<ProductDto>(existingResult.Value);
            _logger.LogInformation("Producto actualizado exitosamente: {ProductId}", id);
            
            return Ok(new { isSuccess = true, data = productDto, message = "Producto actualizado exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar producto {ProductId}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }


    // DELETE: api/products/{id}
    // Remove a product (Administrators only)

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "RequireAdminRole")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _service.DeleteAsync(id);
            if (!result.IsSuccess)
                return NotFound(new { message = $"Product with ID {id} not found" });

            _logger.LogInformation("Product successfully removed: {ProductId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product {ProductId}", id);
            return StatusCode(500, new { message = "Internal Server Error" });
        }
    }
    
    // GET: api/products/count
    // It obtains the total number of products in the system

    [HttpGet("count")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
    public async Task<IActionResult> Count()
    {
        try
        {
            var result = await _service.CountAsync();
            if (!result.IsSuccess)
                return BadRequest(result);
            
            return Ok(new { isSuccess = true, data = result.Value, message = "Count successfully completed" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting products");
            return StatusCode(500, new { message = "Internal Server Error" });
        }
    }
}
