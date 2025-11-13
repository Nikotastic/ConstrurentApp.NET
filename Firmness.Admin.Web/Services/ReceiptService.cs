using Firmness.Application.Interfaces;
using Firmness.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Firmness.Web.Services;

public class ReceiptService : IReceiptService
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;
    private const decimal IVA_RATE = 0.19m; // 19% IVA

    public ReceiptService(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<string> GenerateReceiptAsync(Guid saleId)
    {
        var sale = await _context.Sales
            .Include(s => s.Customer)
            .Include(s => s.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(s => s.Id == saleId);

        if (sale == null)
            throw new InvalidOperationException($"Sale with ID {saleId} not found.");

        // Crear directorio si no existe
        var receiptsPath = Path.Combine(_environment.WebRootPath, "recibos");
        if (!Directory.Exists(receiptsPath))
        {
            Directory.CreateDirectory(receiptsPath);
        }

        var fileName = $"recibo_{saleId}.pdf";
        var filePath = Path.Combine(receiptsPath, fileName);

        // Generar PDF
        var document = CreateReceiptDocument(sale);
        document.GeneratePdf(filePath);

        return filePath;
    }

    public async Task<byte[]> GetReceiptBytesAsync(Guid saleId)
    {
        var sale = await _context.Sales
            .Include(s => s.Customer)
            .Include(s => s.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(s => s.Id == saleId);

        if (sale == null)
            throw new InvalidOperationException($"Sale with ID {saleId} not found.");

        var document = CreateReceiptDocument(sale);
        return document.GeneratePdf();
    }

    public bool ReceiptExists(Guid saleId)
    {
        var receiptsPath = Path.Combine(_environment.WebRootPath, "recibos");
        var fileName = $"recibo_{saleId}.pdf";
        var filePath = Path.Combine(receiptsPath, fileName);
        return File.Exists(filePath);
    }

    public string GetReceiptPath(Guid saleId)
    {
        var receiptsPath = Path.Combine(_environment.WebRootPath, "recibos");
        var fileName = $"recibo_{saleId}.pdf";
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
                    text.Span("Generado el: ");
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
                    col.Item().Text("RECIBO DE VENTA").FontSize(24).SemiBold().FontColor(Colors.Blue.Darken2);
                    col.Item().Text("Firmness - Sistema de Gestión").FontSize(12).FontColor(Colors.Grey.Darken1);
                });
            });

            column.Item().PaddingTop(10);
        });
    }

    private void ComposeContent(IContainer container, Domain.Entities.Sale sale, decimal subtotal, decimal iva, decimal total)
    {
        container.PaddingVertical(20).Column(column =>
        {
            // Información del recibo
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("INFORMACIÓN DEL RECIBO").SemiBold().FontSize(12).FontColor(Colors.Blue.Medium);
                    col.Item().PaddingTop(5).Text(text =>
                    {
                        text.Span("Número de Venta: ").SemiBold();
                        text.Span(sale.Id.ToString().Substring(0, 8).ToUpper());
                    });
                    col.Item().Text(text =>
                    {
                        text.Span("Fecha: ").SemiBold();
                        text.Span(sale.CreatedAt.ToString("dd/MM/yyyy HH:mm"));
                    });
                });

                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("DATOS DEL CLIENTE").SemiBold().FontSize(12).FontColor(Colors.Blue.Medium);
                    col.Item().PaddingTop(5).Text(text =>
                    {
                        text.Span("Cliente: ").SemiBold();
                        text.Span(sale.Customer?.FullName ?? "N/A");
                    });
                    col.Item().Text(text =>
                    {
                        text.Span("Email: ").SemiBold();
                        text.Span(sale.Customer?.Email ?? "N/A");
                    });
                    if (!string.IsNullOrEmpty(sale.Customer?.Document))
                    {
                        col.Item().Text(text =>
                        {
                            text.Span("Documento: ").SemiBold();
                            text.Span(sale.Customer.Document);
                        });
                    }
                    if (!string.IsNullOrEmpty(sale.Customer?.Phone))
                    {
                        col.Item().Text(text =>
                        {
                            text.Span("Teléfono: ").SemiBold();
                            text.Span(sale.Customer.Phone);
                        });
                    }
                });
            });

            column.Item().PaddingVertical(20).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

            // Tabla de productos
            column.Item().Text("DETALLE DE PRODUCTOS").SemiBold().FontSize(12).FontColor(Colors.Blue.Medium);
            
            column.Item().PaddingTop(10).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(40);  // Cantidad
                    columns.RelativeColumn(3);    // Producto
                    columns.RelativeColumn(4);    // Descripción
                    columns.RelativeColumn(2);    // Precio Unit
                    columns.RelativeColumn(2);    // Total
                });

                // Encabezado
                table.Header(header =>
                {
                    header.Cell().Element(HeaderCellStyle).Text("Cant.");
                    header.Cell().Element(HeaderCellStyle).Text("Producto");
                    header.Cell().Element(HeaderCellStyle).Text("Descripción");
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("Precio Unit.");
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("Total");

                    static IContainer HeaderCellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold())
                            .Background(Colors.Grey.Lighten3)
                            .BorderBottom(1)
                            .BorderColor(Colors.Black)
                            .Padding(5);
                    }
                });

                // Filas de productos
                if (sale.Items != null && sale.Items.Any())
                {
                    foreach (var item in sale.Items)
                    {
                        table.Cell().Element(CellStyle).Text(item.Quantity.ToString());
                        table.Cell().Element(CellStyle).Text(item.Product?.Name ?? "N/A");
                        table.Cell().Element(CellStyle).Text(item.Product?.Description ?? "");
                        table.Cell().Element(CellStyle).AlignRight().Text($"${item.UnitPrice:N2}");
                        table.Cell().Element(CellStyle).AlignRight().Text($"${item.LineaTotal:N2}");
                    }
                }

                static IContainer CellStyle(IContainer container)
                {
                    return container.BorderBottom(1)
                        .BorderColor(Colors.Grey.Lighten2)
                        .Padding(5);
                }
            });

            column.Item().PaddingTop(20);

            // Totales
            column.Item().AlignRight().Column(totalsColumn =>
            {
                totalsColumn.Item().Row(row =>
                {
                    row.ConstantItem(150).Text("Subtotal:").SemiBold();
                    row.ConstantItem(100).AlignRight().Text($"${subtotal:N2}");
                });

                totalsColumn.Item().Row(row =>
                {
                    row.ConstantItem(150).Text($"IVA ({IVA_RATE:P0}):").SemiBold();
                    row.ConstantItem(100).AlignRight().Text($"${iva:N2}");
                });

                totalsColumn.Item().PaddingTop(5).BorderTop(2).BorderColor(Colors.Blue.Medium).PaddingTop(5).Row(row =>
                {
                    row.ConstantItem(150).Text("TOTAL:").FontSize(14).SemiBold().FontColor(Colors.Blue.Darken2);
                    row.ConstantItem(100).AlignRight().Text($"${total:N2}").FontSize(14).SemiBold().FontColor(Colors.Blue.Darken2);
                });
            });

            column.Item().PaddingTop(30).BorderTop(1).BorderColor(Colors.Grey.Lighten1).PaddingTop(10)
                .Text("Gracias por su compra").FontSize(10).Italic().FontColor(Colors.Grey.Darken1).AlignCenter();
        });
    }
}

