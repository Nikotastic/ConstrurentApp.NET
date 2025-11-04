using System.ComponentModel.DataAnnotations;

namespace Firmness.Admin.Web.ViewModels;

public class ProductViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "El nombre es requerido.")]
    [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "SKU es requerido.")]
    [StringLength(50, ErrorMessage = "El SKU no puede exceder 50 caracteres.")]
    public string SKU { get; set; } = string.Empty;

    // Price como ejemplo
    [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser un número no negativo.")]
    public decimal Price { get; set; }

    // Campo Age representado como string en el form para mostrar validación de conversión
    // (ejemplo solicitado: validar que la edad ingresada sea un número entero)
    [Display(Name = "Edad (años)")]
    public string? Age { get; set; }

    // Si prefieres mantener Age como int nullable en la entidad, puedes mapearlo desde este campo
    public int? AgeValue
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Age)) return null;
            if (int.TryParse(Age, out var v)) return v;
            return null;
        }
    }
}