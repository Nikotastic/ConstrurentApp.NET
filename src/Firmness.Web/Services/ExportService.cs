using Firmness.Application.Interfaces;
using Firmness.Domain.Interfaces;
using OfficeOpenXml;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Firmness.Web.Services;

public class ExportService : IExportService
{
    private readonly IProductRepository _productRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ISaleRepository _saleRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IVehicleRentalRepository _rentalRepository;
    
    public ExportService(
        IProductRepository productRepository,
        ICustomerRepository customerRepository,
        ISaleRepository saleRepository,
        IVehicleRepository vehicleRepository,
        IVehicleRentalRepository rentalRepository)
    {
        _productRepository = productRepository;
        _customerRepository = customerRepository;
        _saleRepository = saleRepository;
        _vehicleRepository = vehicleRepository;
        _rentalRepository = rentalRepository;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        QuestPDF.Settings.License = LicenseType.Community;
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

        worksheet.DefaultColWidth = 25;
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

        worksheet.DefaultColWidth = 25;
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

        worksheet.DefaultColWidth = 25;
        return package.GetAsByteArray();
    }

    // Export products to PDF
    public async Task<byte[]> ExportProductsToPdfAsync(Guid? categoryId = null)
    {
        var products = await _productRepository.GetAllWithCategoryAsync(categoryId);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header()
                    .Text("Products Report")
                    .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(80);
                            columns.RelativeColumn();
                            columns.ConstantColumn(100);
                            columns.ConstantColumn(80);
                            columns.ConstantColumn(60);
                            columns.ConstantColumn(60);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("SKU");
                            header.Cell().Element(CellStyle).Text("Name");
                            header.Cell().Element(CellStyle).Text("Category");
                            header.Cell().Element(CellStyle).Text("Price");
                            header.Cell().Element(CellStyle).Text("Stock");
                            header.Cell().Element(CellStyle).Text("Status");

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
                            table.Cell().Element(CellStyle).Text(product.Category?.Name ?? "N/A");
                            table.Cell().Element(CellStyle).Text($"${product.Price:N2}");
                            table.Cell().Element(CellStyle).Text(product.Stock.ToString());
                            table.Cell().Element(CellStyle).Text(product.IsActive ? "Active" : "Inactive");

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
                        x.Span("Page ");
                        x.CurrentPageNumber();
                        x.Span(" of ");
                        x.TotalPages();
                    });
            });
        });

        return document.GeneratePdf();
    }

    // Export customers to PDF
    public async Task<byte[]> ExportCustomersToPdfAsync()
    {
        var customers = await _customerRepository.GetAllAsync();
        var orderedCustomers = customers.OrderBy(c => c.LastName).ThenBy(c => c.FirstName).ToList();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header()
                    .Text("Customers Report")
                    .SemiBold().FontSize(20).FontColor(Colors.Green.Medium);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn(2);
                            columns.ConstantColumn(100);
                            columns.ConstantColumn(100);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("First Name");
                            header.Cell().Element(CellStyle).Text("Last Name");
                            header.Cell().Element(CellStyle).Text("Email");
                            header.Cell().Element(CellStyle).Text("Document");
                            header.Cell().Element(CellStyle).Text("Phone");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.DefaultTextStyle(x => x.SemiBold())
                                    .PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                            }
                        });

                        foreach (var customer in orderedCustomers)
                        {
                            table.Cell().Element(CellStyle).Text(customer.FirstName);
                            table.Cell().Element(CellStyle).Text(customer.LastName);
                            table.Cell().Element(CellStyle).Text(customer.Email ?? "N/A");
                            table.Cell().Element(CellStyle).Text(customer.Document ?? "N/A");
                            table.Cell().Element(CellStyle).Text(customer.Phone ?? "N/A");

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
                        x.Span("Page ");
                        x.CurrentPageNumber();
                        x.Span(" of ");
                        x.TotalPages();
                    });
            });
        });

        return document.GeneratePdf();
    }

    // Export sales to PDF
    public async Task<byte[]> ExportSalesToPdfAsync()
    {
        var sales = await _saleRepository.GetAllWithDetailsAsync();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header()
                    .Text("Sales Report")
                    .SemiBold().FontSize(20).FontColor(Colors.Red.Medium);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(100);
                            columns.ConstantColumn(80);
                            columns.RelativeColumn();
                            columns.ConstantColumn(80);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Date");
                            header.Cell().Element(CellStyle).Text("Invoice");
                            header.Cell().Element(CellStyle).Text("Customer");
                            header.Cell().Element(CellStyle).Text("Total");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.DefaultTextStyle(x => x.SemiBold())
                                    .PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                            }
                        });

                        foreach (var sale in sales)
                        {
                            table.Cell().Element(CellStyle).Text(sale.CreatedAt.ToString("dd/MM/yyyy"));
                            table.Cell().Element(CellStyle).Text(sale.InvoiceNumber ?? "N/A");
                            table.Cell().Element(CellStyle).Text(sale.Customer?.FullName ?? "N/A");
                            table.Cell().Element(CellStyle).Text($"${sale.TotalAmount:N2}");

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
                        x.Span("Page ");
                        x.CurrentPageNumber();
                        x.Span(" of ");
                        x.TotalPages();
                    });
            });
        });

        return document.GeneratePdf();
    }

    // Export vehicles to Excel
    public async Task<byte[]> ExportVehiclesToExcelAsync()
    {
        var vehicles = await _vehicleRepository.GetAllAsync();

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Vehicles");

        // Headers
        worksheet.Cells[1, 1].Value = "Brand";
        worksheet.Cells[1, 2].Value = "Model";
        worksheet.Cells[1, 3].Value = "Year";
        worksheet.Cells[1, 4].Value = "License Plate";
        worksheet.Cells[1, 5].Value = "Type";
        worksheet.Cells[1, 6].Value = "Daily Rate";
        worksheet.Cells[1, 7].Value = "Status";
        worksheet.Cells[1, 8].Value = "Hours";

        // Style headers
        using (var range = worksheet.Cells[1, 1, 1, 8])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightCoral);
        }

        // Data
        int row = 2;
        foreach (var vehicle in vehicles)
        {
            worksheet.Cells[row, 1].Value = vehicle.Brand;
            worksheet.Cells[row, 2].Value = vehicle.Model;
            worksheet.Cells[row, 3].Value = vehicle.Year;
            worksheet.Cells[row, 4].Value = vehicle.LicensePlate;
            worksheet.Cells[row, 5].Value = vehicle.VehicleType.ToString();
            worksheet.Cells[row, 6].Value = vehicle.DailyRate;
            worksheet.Cells[row, 6].Style.Numberformat.Format = "$#,##0.00";
            worksheet.Cells[row, 7].Value = vehicle.Status.ToString();
            worksheet.Cells[row, 8].Value = vehicle.CurrentHours;
            row++;
        }

        worksheet.DefaultColWidth = 25;
        return package.GetAsByteArray();
    }

    // Export vehicles to PDF
    public async Task<byte[]> ExportVehiclesToPdfAsync()
    {
        var vehicles = await _vehicleRepository.GetAllAsync();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header()
                    .Text("Vehicles Report")
                    .SemiBold().FontSize(20).FontColor(Colors.Orange.Medium);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.ConstantColumn(60);
                            columns.ConstantColumn(100);
                            columns.ConstantColumn(100);
                            columns.ConstantColumn(80);
                            columns.ConstantColumn(80);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Brand");
                            header.Cell().Element(CellStyle).Text("Model");
                            header.Cell().Element(CellStyle).Text("Year");
                            header.Cell().Element(CellStyle).Text("License Plate");
                            header.Cell().Element(CellStyle).Text("Type");
                            header.Cell().Element(CellStyle).Text("Daily Rate");
                            header.Cell().Element(CellStyle).Text("Status");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.DefaultTextStyle(x => x.SemiBold())
                                    .PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                            }
                        });

                        foreach (var vehicle in vehicles)
                        {
                            table.Cell().Element(CellStyle).Text(vehicle.Brand);
                            table.Cell().Element(CellStyle).Text(vehicle.Model);
                            table.Cell().Element(CellStyle).Text(vehicle.Year.ToString());
                            table.Cell().Element(CellStyle).Text(vehicle.LicensePlate);
                            table.Cell().Element(CellStyle).Text(vehicle.VehicleType.ToString());
                            table.Cell().Element(CellStyle).Text($"${vehicle.DailyRate:N2}");
                            table.Cell().Element(CellStyle).Text(vehicle.Status.ToString());

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
                        x.Span("Page ");
                        x.CurrentPageNumber();
                        x.Span(" of ");
                        x.TotalPages();
                    });
            });
        });

        return document.GeneratePdf();
    }

    // Export vehicle rentals to Excel
    public async Task<byte[]> ExportVehicleRentalsToExcelAsync()
    {
        var rentals = await _rentalRepository.GetAllWithDetailsAsync();

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Vehicle Rentals");

        // Headers
        worksheet.Cells[1, 1].Value = "Customer";
        worksheet.Cells[1, 2].Value = "Vehicle";
        worksheet.Cells[1, 3].Value = "Start Date";
        worksheet.Cells[1, 4].Value = "Return Date";
        worksheet.Cells[1, 5].Value = "Status";
        worksheet.Cells[1, 6].Value = "Total Amount";
        worksheet.Cells[1, 7].Value = "Paid";
        worksheet.Cells[1, 8].Value = "Pending";

        // Style headers
        using (var range = worksheet.Cells[1, 1, 1, 8])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightSkyBlue);
        }

        // Data
        int row = 2;
        foreach (var rental in rentals)
        {
            worksheet.Cells[row, 1].Value = rental.Customer?.FullName ?? "N/A";
            worksheet.Cells[row, 2].Value = rental.Vehicle?.DisplayName ?? "N/A";
            worksheet.Cells[row, 3].Value = rental.StartDate.ToString("dd/MM/yyyy");
            worksheet.Cells[row, 4].Value = (rental.ActualReturnDate ?? rental.EstimatedReturnDate).ToString("dd/MM/yyyy");
            worksheet.Cells[row, 5].Value = rental.Status.ToString();
            worksheet.Cells[row, 6].Value = rental.TotalAmount;
            worksheet.Cells[row, 6].Style.Numberformat.Format = "$#,##0.00";
            worksheet.Cells[row, 7].Value = rental.PaidAmount;
            worksheet.Cells[row, 7].Style.Numberformat.Format = "$#,##0.00";
            worksheet.Cells[row, 8].Value = rental.PendingAmount;
            worksheet.Cells[row, 8].Style.Numberformat.Format = "$#,##0.00";
            row++;
        }

        worksheet.DefaultColWidth = 25;
        return package.GetAsByteArray();
    }

    // Export vehicle rentals to PDF
    public async Task<byte[]> ExportVehicleRentalsToPdfAsync()
    {
        var rentals = await _rentalRepository.GetAllWithDetailsAsync();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(9));

                page.Header()
                    .Text("Vehicle Rentals Report")
                    .SemiBold().FontSize(20).FontColor(Colors.Indigo.Medium);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.ConstantColumn(80);
                            columns.ConstantColumn(80);
                            columns.ConstantColumn(70);
                            columns.ConstantColumn(80);
                            columns.ConstantColumn(80);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Customer");
                            header.Cell().Element(CellStyle).Text("Vehicle");
                            header.Cell().Element(CellStyle).Text("Start Date");
                            header.Cell().Element(CellStyle).Text("Return Date");
                            header.Cell().Element(CellStyle).Text("Status");
                            header.Cell().Element(CellStyle).Text("Total");
                            header.Cell().Element(CellStyle).Text("Pending");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.DefaultTextStyle(x => x.SemiBold())
                                    .PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                            }
                        });

                        foreach (var rental in rentals)
                        {
                            table.Cell().Element(CellStyle).Text(rental.Customer?.FullName ?? "N/A");
                            table.Cell().Element(CellStyle).Text(rental.Vehicle?.DisplayName ?? "N/A");
                            table.Cell().Element(CellStyle).Text(rental.StartDate.ToString("dd/MM/yy"));
                            table.Cell().Element(CellStyle).Text((rental.ActualReturnDate ?? rental.EstimatedReturnDate).ToString("dd/MM/yy"));
                            table.Cell().Element(CellStyle).Text(rental.Status.ToString());
                            table.Cell().Element(CellStyle).Text($"${rental.TotalAmount:N2}");
                            table.Cell().Element(CellStyle).Text($"${rental.PendingAmount:N2}");

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
                        x.Span("Page ");
                        x.CurrentPageNumber();
                        x.Span(" of ");
                        x.TotalPages();
                    });
            });
        });

        return document.GeneratePdf();
    }
}
