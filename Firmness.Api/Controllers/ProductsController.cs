using Firmness.Application.Interfaces;
using Firmness.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Firmness.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Requiere autenticación JWT para todos los endpoints
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService service, ILogger<ProductsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous] // Este endpoint es público (no requiere token)
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        var result = await _service.GetAllAsync(page, pageSize);
        if (!result.IsSuccess)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("search")]
    [AllowAnonymous] // Búsqueda pública
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        var result = await _service.SearchAsync(q, page, pageSize);
        if (!result.IsSuccess)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous] // Ver detalle de producto es público
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (!result.IsSuccess)
            return NotFound(result);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "RequireAdminRole")] // Solo admins pueden crear productos
    public async Task<IActionResult> Create([FromBody] Product product)
    {
        var result = await _service.AddAsync(product);
        if (!result.IsSuccess)
            return BadRequest(result);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "RequireAdminRole")] // Solo admins pueden editar
    public async Task<IActionResult> Update(Guid id, [FromBody] Product product)
    {
        if (id != product.Id) return BadRequest("Id mismatch");
        var result = await _service.UpdateAsync(product);
        if (!result.IsSuccess)
            return BadRequest(result);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "RequireAdminRole")] // Solo admins pueden eliminar
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _service.DeleteAsync(id);
        if (!result.IsSuccess)
            return BadRequest(result);
        return NoContent();
    }

    [HttpGet("count")]
    [AllowAnonymous] // Contador público
    public async Task<IActionResult> Count()
    {
        var result = await _service.CountAsync();
        if (!result.IsSuccess)
            return BadRequest(result);
        return Ok(result);
    }
}
