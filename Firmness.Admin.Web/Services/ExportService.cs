using Firmness.Application.Interfaces;
using Firmness.Domain.Interfaces;
using OfficeOpenXml;

namespace Firmness.Web.Services;

public class ExportService : IExportService
{
    private readonly IProductRepository _productRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ISaleRepository _saleRepository;
    
    public ExportService(
        IProductRepository productRepository,
        ICustomerRepository customerRepository,
        ISaleRepository saleRepository)
    {
        _productRepository = productRepository;
        _customerRepository = customerRepository;
        _saleRepository = saleRepository;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }
    
    // Export products to Excel
    public async Task<byte[]> ExportProductsToExcelAsync(Guid? categoryId = null)
    {
        var products = await _productRepository.GetAllWithCategoryAsync(categoryId);

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Products");

        // Headers
        worksheet.Cells[1, 1].Value = "SKU";
        worksheet.Cells[1, 2].Value = "Name";
        worksheet.Cells[1, 3].Value = "Category";
        worksheet.Cells[1, 4].Value = "Description";
        worksheet.Cells[1, 5].Value = "Price";
        worksheet.Cells[1, 6].Value = "Cost";
        worksheet.Cells[1, 7].Value = "Stock";
        worksheet.Cells[1, 8].Value = "Min Stock";
        worksheet.Cells[1, 9].Value = "Barcode";
        worksheet.Cells[1, 10].Value = "Status";

        // Style headers
        using (var range = worksheet.Cells[1, 1, 1, 10])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        }

        // Data
        int row = 2;
        foreach (var product in products)
        {
            worksheet.Cells[row, 1].Value = product.SKU;
            worksheet.Cells[row, 2].Value = product.Name;
            worksheet.Cells[row, 3].Value = product.Category?.Name ?? "No Category";
            worksheet.Cells[row, 4].Value = product.Description;
            worksheet.Cells[row, 5].Value = product.Price;
            worksheet.Cells[row, 5].Style.Numberformat.Format = "$#,##0.00";
            worksheet.Cells[row, 6].Value = product.Cost;
            worksheet.Cells[row, 6].Style.Numberformat.Format = "$#,##0.00";
            worksheet.Cells[row, 7].Value = product.Stock;
            worksheet.Cells[row, 8].Value = product.MinStock;
            worksheet.Cells[row, 9].Value = product.Barcode ?? "";
            worksheet.Cells[row, 10].Value = product.IsActive ? "Active" : "Inactive";
            row++;
        }

        worksheet.Cells.AutoFitColumns();
        return package.GetAsByteArray();
    }

    // Export customers to Excel
    public async Task<byte[]> ExportCustomersToExcelAsync()
    {
        var customers = await _customerRepository.GetAllAsync();
        var orderedCustomers = customers.OrderBy(c => c.LastName).ThenBy(c => c.FirstName);

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Clientes");

        // Headers
        worksheet.Cells[1, 1].Value = "Nombre";
        worksheet.Cells[1, 2].Value = "Apellido";
        worksheet.Cells[1, 3].Value = "Email";
        worksheet.Cells[1, 4].Value = "Documento";
        worksheet.Cells[1, 5].Value = "Teléfono";
        worksheet.Cells[1, 6].Value = "Dirección";
        worksheet.Cells[1, 7].Value = "Estado";

        // Style headers
        using (var range = worksheet.Cells[1, 1, 1, 7])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);
        }

        // Data
        int row = 2;
        foreach (var customer in orderedCustomers)
        {
            worksheet.Cells[row, 1].Value = customer.FirstName;
            worksheet.Cells[row, 2].Value = customer.LastName;
            worksheet.Cells[row, 3].Value = customer.Email;
            worksheet.Cells[row, 4].Value = customer.Document;
            worksheet.Cells[row, 5].Value = customer.Phone;
            worksheet.Cells[row, 6].Value = customer.Address;
            worksheet.Cells[row, 7].Value = customer.IsActive ? "Activo" : "Inactivo";
            row++;
        }

        worksheet.Cells.AutoFitColumns();
        return package.GetAsByteArray();
    }

    // Export sales to Excel
    public async Task<byte[]> ExportSalesToExcelAsync()
    {
        var sales = await _saleRepository.GetAllWithDetailsAsync();

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Ventas");

        // headers
        worksheet.Cells[1, 1].Value = "Número Venta";
        worksheet.Cells[1, 2].Value = "Nº Factura";
        worksheet.Cells[1, 3].Value = "Fecha";
        worksheet.Cells[1, 4].Value = "Cliente";
        worksheet.Cells[1, 5].Value = "Total";
        worksheet.Cells[1, 6].Value = "Cantidad Items";

        // Style headers
        using (var range = worksheet.Cells[1, 1, 1, 6])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);
        }

        // Data
        int row = 2;
        foreach (var sale in sales)
        {
            worksheet.Cells[row, 1].Value = sale.Id.ToString().Substring(0, 8).ToUpper();
            worksheet.Cells[row, 2].Value = sale.InvoiceNumber ?? "N/A";
            worksheet.Cells[row, 3].Value = sale.CreatedAt.ToString("dd/MM/yyyy HH:mm");
            worksheet.Cells[row, 4].Value = sale.Customer?.FullName ?? "N/A";
            worksheet.Cells[row, 5].Value = sale.TotalAmount;
            worksheet.Cells[row, 5].Style.Numberformat.Format = "$#,##0.00";
            worksheet.Cells[row, 6].Value = sale.Items?.Count ?? 0;
            row++;
        }

        worksheet.Cells.AutoFitColumns();
        return package.GetAsByteArray();
    }

    // Export products to PDF - Placeholder (to be implemented)
    public Task<byte[]> ExportProductsToPdfAsync(Guid? categoryId = null)
    {
        // TODO: Implement PDF export using QuestPDF with repositories
        throw new NotImplementedException("PDF export for products will be implemented using repositories");
    }

    // Export customers to PDF - Placeholder (to be implemented)
    public Task<byte[]> ExportCustomersToPdfAsync()
    {
        // TODO: Implement PDF export using QuestPDF with repositories
        throw new NotImplementedException("PDF export for customers will be implemented using repositories");
    }

    // Export sales to PDF - Placeholder (to be implemented)
    public Task<byte[]> ExportSalesToPdfAsync()
    {
        // TODO: Implement PDF export using QuestPDF with repositories
        throw new NotImplementedException("PDF export for sales will be implemented using repositories");
    }
}
