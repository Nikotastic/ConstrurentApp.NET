using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Firmness.Admin.Web.ViewModels.Sales;

public class SaleFormViewModel
{
    public Guid Id { get; set; }
    
    [Required(ErrorMessage = "Select a customer.")]
    [Display(Name = "Customer")]
    public Guid CustomerId { get; set; }
    
    public IEnumerable<SelectListItem> Customers { get; set; } = new List<SelectListItem>();
    
    
    [Required(ErrorMessage = "Date is required.")]
    [DataType(DataType.Date)]
    [Display(Name = "Date")]
    public DateTime CreatedAt { get; set; } = DateTime.Today;
    
    [Required(ErrorMessage = "Enter the total amount.")]
    [Display(Name = "Total Amount")]
    [DataType(DataType.Currency)]
    [Range(0, double.MaxValue, ErrorMessage = "Total amount must be greater than or equal to 0.")]
    public decimal TotalAmount { get; set; }
    
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    
}