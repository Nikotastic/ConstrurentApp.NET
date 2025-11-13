using Firmness.Application.Interfaces;
using Firmness.Domain.Entities;
using Firmness.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Firmness.Web.Services;

public class ExportService : IExportService
{
    private readonly ApplicationDbContext _context;

    public ExportService(ApplicationDbContext context)
    {
        _context = context;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> ExportProductsToExcelAsync()
    {
        var products = await _context.Products
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .ToListAsync();

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Productos");

        // Encabezados
        worksheet.Cells[1, 1].Value = "SKU";
        worksheet.Cells[1, 2].Value = "Nombre";
        worksheet.Cells[1, 3].Value = "Descripción";
        worksheet.Cells[1, 4].Value = "Precio";
        worksheet.Cells[1, 5].Value = "Stock";

        // Estilo de encabezados
        using (var range = worksheet.Cells[1, 1, 1, 5])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
        }

        // Datos
        int row = 2;
        foreach (var product in products)
        {
            worksheet.Cells[row, 1].Value = product.SKU;
            worksheet.Cells[row, 2].Value = product.Name;
            worksheet.Cells[row, 3].Value = product.Description;
            worksheet.Cells[row, 4].Value = product.Price;
            worksheet.Cells[row, 4].Style.Numberformat.Format = "$#,##0.00";
            worksheet.Cells[row, 5].Value = product.Stock;
            row++;
        }

        worksheet.Cells.AutoFitColumns();
        return package.GetAsByteArray();
    }

    public async Task<byte[]> ExportCustomersToExcelAsync()
    {
        var customers = await _context.Customers
            .AsNoTracking()
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .ToListAsync();

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Clientes");

        // Encabezados
        worksheet.Cells[1, 1].Value = "Nombre";
        worksheet.Cells[1, 2].Value = "Apellido";
        worksheet.Cells[1, 3].Value = "Email";
        worksheet.Cells[1, 4].Value = "Documento";
        worksheet.Cells[1, 5].Value = "Teléfono";
        worksheet.Cells[1, 6].Value = "Dirección";
        worksheet.Cells[1, 7].Value = "Estado";

        // Estilo de encabezados
        using (var range = worksheet.Cells[1, 1, 1, 7])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);
        }

        // Datos
        int row = 2;
        foreach (var customer in customers)
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

    public async Task<byte[]> ExportSalesToExcelAsync()
    {
        var sales = await _context.Sales
            .AsNoTracking()
            .Include(s => s.Customer)
            .Include(s => s.Items)
                .ThenInclude(i => i.Product)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Ventas");

        // Encabezados
        worksheet.Cells[1, 1].Value = "Número Venta";
        worksheet.Cells[1, 2].Value = "Fecha";
        worksheet.Cells[1, 3].Value = "Cliente";
        worksheet.Cells[1, 4].Value = "Email Cliente";
        worksheet.Cells[1, 5].Value = "Total";
        worksheet.Cells[1, 6].Value = "Cantidad Items";

        // Estilo de encabezados
        using (var range = worksheet.Cells[1, 1, 1, 6])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);
        }

        // Datos
        int row = 2;
        foreach (var sale in sales)
        {
            worksheet.Cells[row, 1].Value = sale.Id.ToString().Substring(0, 8).ToUpper();
            worksheet.Cells[row, 2].Value = sale.CreatedAt;
            worksheet.Cells[row, 2].Style.Numberformat.Format = "dd/MM/yyyy HH:mm";
            worksheet.Cells[row, 3].Value = sale.Customer?.FullName ?? "N/A";
            worksheet.Cells[row, 4].Value = sale.Customer?.Email ?? "N/A";
            worksheet.Cells[row, 5].Value = sale.TotalAmount;
            worksheet.Cells[row, 5].Style.Numberformat.Format = "$#,##0.00";
            worksheet.Cells[row, 6].Value = sale.Items?.Count ?? 0;
            row++;
        }

        worksheet.Cells.AutoFitColumns();
        return package.GetAsByteArray();
    }

    public async Task<byte[]> ExportProductsToPdfAsync()
    {
        var products = await _context.Products
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .ToListAsync();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header()
                    .Text("Listado de Productos")
                    .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(4);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("SKU");
                            header.Cell().Element(CellStyle).Text("Nombre");
                            header.Cell().Element(CellStyle).Text("Descripción");
                            header.Cell().Element(CellStyle).Text("Precio");
                            header.Cell().Element(CellStyle).Text("Stock");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.DefaultTextStyle(x => x.SemiBold())
                                    .PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                            }
                        });

                        foreach (var product in products)
                        {
                            table.Cell().Element(CellStyle).Text(product.SKU);
                            table.Cell().Element(CellStyle).Text(product.Name);
                            table.Cell().Element(CellStyle).Text(product.Description ?? "");
                            table.Cell().Element(CellStyle).Text($"${product.Price:N2}");
                            table.Cell().Element(CellStyle).Text(product.Stock.ToString());

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                    .PaddingVertical(5);
                            }
                        }
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
            });
        });

        return document.GeneratePdf();
    }

    public async Task<byte[]> ExportCustomersToPdfAsync()
    {
        var customers = await _context.Customers
            .AsNoTracking()
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .ToListAsync();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header()
                    .Text("Listado de Clientes")
                    .SemiBold().FontSize(20).FontColor(Colors.Green.Medium);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Nombre");
                            header.Cell().Element(CellStyle).Text("Apellido");
                            header.Cell().Element(CellStyle).Text("Email");
                            header.Cell().Element(CellStyle).Text("Documento");
                            header.Cell().Element(CellStyle).Text("Teléfono");
                            header.Cell().Element(CellStyle).Text("Estado");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.DefaultTextStyle(x => x.SemiBold())
                                    .PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                            }
                        });

                        foreach (var customer in customers)
                        {
                            table.Cell().Element(CellStyle).Text(customer.FirstName);
                            table.Cell().Element(CellStyle).Text(customer.LastName);
                            table.Cell().Element(CellStyle).Text(customer.Email);
                            table.Cell().Element(CellStyle).Text(customer.Document ?? "");
                            table.Cell().Element(CellStyle).Text(customer.Phone ?? "");
                            table.Cell().Element(CellStyle).Text(customer.IsActive ? "Activo" : "Inactivo");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                    .PaddingVertical(5);
                            }
                        }
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
            });
        });

        return document.GeneratePdf();
    }

    public async Task<byte[]> ExportSalesToPdfAsync()
    {
        var sales = await _context.Sales
            .AsNoTracking()
            .Include(s => s.Customer)
            .Include(s => s.Items)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header()
                    .Text("Listado de Ventas")
                    .SemiBold().FontSize(20).FontColor(Colors.Orange.Medium);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Número");
                            header.Cell().Element(CellStyle).Text("Fecha");
                            header.Cell().Element(CellStyle).Text("Cliente");
                            header.Cell().Element(CellStyle).Text("Total");
                            header.Cell().Element(CellStyle).Text("Items");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.DefaultTextStyle(x => x.SemiBold())
                                    .PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                            }
                        });

                        foreach (var sale in sales)
                        {
                            table.Cell().Element(CellStyle).Text(sale.Id.ToString().Substring(0, 8).ToUpper());
                            table.Cell().Element(CellStyle).Text(sale.CreatedAt.ToString("dd/MM/yyyy"));
                            table.Cell().Element(CellStyle).Text(sale.Customer?.FullName ?? "N/A");
                            table.Cell().Element(CellStyle).Text($"${sale.TotalAmount:N2}");
                            table.Cell().Element(CellStyle).Text((sale.Items?.Count ?? 0).ToString());

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                    .PaddingVertical(5);
                            }
                        }
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
            });
        });

        return document.GeneratePdf();
    }
}

