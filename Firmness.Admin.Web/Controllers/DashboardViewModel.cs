using Microsoft.AspNetCore.Mvc;

namespace Firmness.Admin.Web.Controllers;

public class DashboardViewModel
{
    public long TotalProducts { get; set; }
    public long TotalCustomers { get; set; }
    public long TotalSales { get; set; }
}