using Firmness.Application.Interfaces;
using Firmness.Domain.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Firmness.Web.Services;

public class ReceiptService : IReceiptService
{
    private readonly ISaleRepository _saleRepository;
    private readonly IWebHostEnvironment _environment;
    private const decimal IVA_RATE = 0.19m; // 19% IVA

    public ReceiptService(ISaleRepository saleRepository, IWebHostEnvironment environment)
    {
        _saleRepository = saleRepository;
        _environment = environment;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<string> GenerateReceiptAsync(Guid saleId)
    {
        var sale = await _saleRepository.GetByIdWithDetailsAsync(saleId);

        if (sale == null)
            throw new InvalidOperationException($"Sale with ID {saleId} not found.");

        
        var receiptsPath = Path.Combine(_environment.WebRootPath, "receipts");
        if (!Directory.Exists(receiptsPath))
        {
            Directory.CreateDirectory(receiptsPath);
        }

        var fileName = $"receipts_{saleId}.pdf";
        var filePath = Path.Combine(receiptsPath, fileName);

        // Generar PDF
        var document = CreateReceiptDocument(sale);
        document.GeneratePdf(filePath);

        return filePath;
    }

    public async Task<byte[]> GetReceiptBytesAsync(Guid saleId)
    {
        var sale = await _saleRepository.GetByIdWithDetailsAsync(saleId);

        if (sale == null)
            throw new InvalidOperationException($"Sale with ID {saleId} not found.");

        var document = CreateReceiptDocument(sale);
        return document.GeneratePdf();
    }

    public bool ReceiptExists(Guid saleId)
    {
        var receiptsPath = Path.Combine(_environment.WebRootPath, "receipts");
        var fileName = $"receipts_{saleId}.pdf";
        var filePath = Path.Combine(receiptsPath, fileName);
        return File.Exists(filePath);
    }

    public string GetReceiptPath(Guid saleId)
    {
        var receiptsPath = Path.Combine(_environment.WebRootPath, "receipts");
        var fileName = $"receipts_{saleId}.pdf";
        return Path.Combine(receiptsPath, fileName);
    }

    private Document CreateReceiptDocument(Domain.Entities.Sale sale)
    {
        var subtotal = sale.Items?.Sum(i => i.LineaTotal) ?? 0;
        var iva = subtotal * IVA_RATE;
        var total = subtotal + iva;

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Element(ComposeHeader);
                page.Content().Element(content => ComposeContent(content, sale, subtotal, iva, total));
                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Generated on: ");
                    text.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).SemiBold();
                });
            });
        });
    }

    private void ComposeHeader(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().BorderBottom(2).BorderColor(Colors.Blue.Darken2).PaddingBottom(10).Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("SALES RECEIPT").FontSize(24).SemiBold().FontColor(Colors.Blue.Darken2);
                    col.Item().Text("Firmness - Management System").FontSize(12).FontColor(Colors.Grey.Darken1);
                });
            });

            column.Item().PaddingTop(10);
        });
    }

    private void ComposeContent(IContainer container, Domain.Entities.Sale sale, decimal subtotal, decimal iva, decimal total)
    {
        container.PaddingVertical(20).Column(column =>
        {
            // Receipt information
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("RECEIPT INFORMATION").SemiBold().FontSize(12).FontColor(Colors.Blue.Medium);
                    col.Item().PaddingTop(5).Text(text =>
                    {
                        text.Span("Sales Number: ").SemiBold();
                        text.Span(sale.Id.ToString().Substring(0, 8).ToUpper());
                    });
                    col.Item().Text(text =>
                    {
                        text.Span("Date: ").SemiBold();
                        text.Span(sale.CreatedAt.ToString("dd/MM/yyyy HH:mm"));
                    });
                });

                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("CUSTOMER DATAE").SemiBold().FontSize(12).FontColor(Colors.Blue.Medium);
                    col.Item().PaddingTop(5).Text(text =>
                    {
                        text.Span("Customer: ").SemiBold();
                        text.Span(sale.Customer?.FullName ?? "N/A");
                    });
                    if (!string.IsNullOrEmpty(sale.Customer?.Email))
                    {
                        col.Item().Text(text =>
                        {
                            text.Span("Email: ").SemiBold();
                            text.Span(sale.Customer.Email);
                        });
                    }
                    if (!string.IsNullOrEmpty(sale.Customer?.Phone))
                    {
                        col.Item().Text(text =>
                        {
                            text.Span("Phone: ").SemiBold();
                            text.Span(sale.Customer.Phone);
                        });
                    }
                });
            });
            column.Item().PaddingVertical(15);

            // Product table
            column.Item().Text("PRODUCT DETAILS").SemiBold().FontSize(12).FontColor(Colors.Blue.Medium);
            column.Item().PaddingTop(5).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3); // Product
                    columns.RelativeColumn(1); //Amount
                    columns.RelativeColumn(1); // Unit Price
                    columns.RelativeColumn(1); // Total
                });

                // Header
                table.Header(header =>
                {
                    header.Cell().Background(Colors.Blue.Lighten3).Padding(5).Text("Product").SemiBold();
                    header.Cell().Background(Colors.Blue.Lighten3).Padding(5).Text("Amount").SemiBold();
                    header.Cell().Background(Colors.Blue.Lighten3).Padding(5).Text("Unit Price").SemiBold();
                    header.Cell().Background(Colors.Blue.Lighten3).Padding(5).Text("Total").SemiBold();
                });

                // Items
                if (sale.Items != null)
                {
                    foreach (var item in sale.Items)
                    {
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5)
                            .Text(item.Product?.Name ?? "Unknown product");
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5)
                            .Text(item.Quantity.ToString("N2"));
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5);
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5)
                            .Text($"${item.LineaTotal:N2}");
                    }
                }
            });

            // Totals
            column.Item().PaddingTop(15).AlignRight().Column(col =>
            {
                col.Item().Text(text =>
                {
                    text.Span("Subtotal: ").SemiBold();
                    text.Span($"${subtotal:N2}");
                });
                col.Item().Text(text =>
                {
                    text.Span("IVA (19%): ").SemiBold();
                    text.Span($"${iva:N2}");
                });
                col.Item().PaddingTop(5).Text(text =>
                {
                    text.Span("TOTAL: ").SemiBold().FontSize(14).FontColor(Colors.Blue.Darken2);
                    text.Span($"${total:N2}").FontSize(14).FontColor(Colors.Blue.Darken2);
                });
            });
        });
    }
}
