using System.ComponentModel.DataAnnotations;
using Firmness.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Firmness.Web.ViewModels.Sales;

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
    
    [Required]
    [Display(Name = "Status")]
    public SaleStatus Status { get; set; } = SaleStatus.Pending;
    
    [Required]
    [Display(Name = "Payment Method")]
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
    
    [Display(Name = "Subtotal")]
    [DataType(DataType.Currency)]
    public decimal Subtotal { get; set; }
    
    [Display(Name = "Tax")]
    [DataType(DataType.Currency)]
    public decimal Tax { get; set; }
    
    [Display(Name = "Discount")]
    [DataType(DataType.Currency)]
    public decimal Discount { get; set; }
    
    [Display(Name = "Notes")]
    [DataType(DataType.MultilineText)]
    public string? Notes { get; set; }
    
    [Display(Name = "Invoice Number")]
    public string? InvoiceNumber { get; set; }
    
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
}