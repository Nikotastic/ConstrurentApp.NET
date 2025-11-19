using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Firmness.Web.ViewModels.Product;

public class ProductFormViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "The SKU is required")]
    [StringLength(50, ErrorMessage = "The SKU must be less than 50 characters")]
    public string SKU { get; set; } = string.Empty;

    [Required(ErrorMessage = "The name is required.")]
    [StringLength(200, ErrorMessage = "The name must be lees than 200 characters.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "The description must be lees than 1000 characters.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "El precio es requerido")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
    [Display(Name = "Precio de Venta")]
    [DataType(DataType.Currency)]
    public decimal? Price { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "El costo debe ser un número positivo")]
    [Display(Name = "Costo")]
    [DataType(DataType.Currency)]
    public decimal? Cost { get; set; }

    [Display(Name = "Imagen (URL)")]
    [Url(ErrorMessage = "The URL is invalid..")]
    [StringLength(2000, ErrorMessage = "The URL is too long.")]
    public string? ImageUrl { get; set; }

    [Required(ErrorMessage = "El stock es requerido")]
    [Range(0, double.MaxValue, ErrorMessage = "El stock debe ser un número positivo")]
    [Display(Name = "Stock Actual")]
    public decimal? Stock { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "El stock mínimo debe ser un número positivo")]
    [Display(Name = "Stock Mínimo")]
    public decimal? MinStock { get; set; }

    [StringLength(100, ErrorMessage = "El código de barras debe tener menos de 100 caracteres")]
    [Display(Name = "Código de Barras")]
    public string? Barcode { get; set; }

    [Display(Name = "Categoría")]
    public Guid? CategoryId { get; set; }

    [Display(Name = "Activo")]
    public bool IsActive { get; set; } = true;

    // Navigation properties for dropdowns
    public IEnumerable<SelectListItem>? Categories { get; set; }

    // Display properties
    public string? CategoryName { get; set; }
}