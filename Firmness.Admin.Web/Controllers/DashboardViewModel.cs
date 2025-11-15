using Microsoft.AspNetCore.Mvc;
using Firmness.Domain.Entities;

namespace Firmness.Web.Controllers;

public class DashboardViewModel
{
    public long TotalProducts { get; set; }
    public long TotalCustomers { get; set; }
    public long TotalSales { get; set; }
    public decimal TotalRevenue { get; set; }
    public long PendingSales { get; set; }
    public long CompletedSales { get; set; }
    
    public List<Sale> RecentSales { get; set; } = new();
    public List<Product> LowStockProducts { get; set; } = new();
    public List<Customer> TopCustomers { get; set; } = new();
}