using System.ComponentModel.DataAnnotations;

namespace Firmness.Admin.Web.ViewModels.Product;

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

    [Required(ErrorMessage = "The price is required.")]
    [Range(0, double.MaxValue, ErrorMessage = "The price must be a non-negative number..")]
    public decimal? Price { get; set; }

    [Display(Name = "Imagen (URL)")]
    [Url(ErrorMessage = "The URL is invalid..")]
    [StringLength(2000, ErrorMessage = "The URL is too long.")]
    public string? ImageUrl { get; set; }

    [Required(ErrorMessage = "Stock is required.")]
    [Range(0, double.MaxValue, ErrorMessage = "The stock must be a non-negative number..")]
    public decimal? Stock { get; set; }
}