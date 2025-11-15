using System.ComponentModel.DataAnnotations;

namespace Firmness.Web.ViewModels.Category;

// ViewModel for Category forms (Create/Edit)
public class CategoryFormViewModel
{
    public Guid Id { get; set; }
    
    [Required(ErrorMessage = "The name is required")]
    [StringLength(100, ErrorMessage = "The name must be less than 100 characters")]
    [Display(Name = "Name")]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(500, ErrorMessage = "The description must be less than 500 characters")]
    [Display(Name = "Description")]
    public string? Description { get; set; }
    
    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
    
    [Display(Name = "Date created")]
    public DateTime? CreatedAt { get; set; }
    
    [Display(Name = "Last Update")]
    public DateTime? UpdatedAt { get; set; }
    
    [Display(Name = "Count products")]
    public int ProductCount { get; set; }
}

