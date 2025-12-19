using Firmness.Application.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Firmness.Infrastructure.Pdf;

public class QuestPdfService : IPdfService
{
    public QuestPdfService()
    {
        // Configure QuestPDF license (Community for non-commercial use)
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerateReceiptPdf(ReceiptData data)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Element(ComposeHeader);
                page.Content().Element(c => ComposeContent(c, data));
                page.Footer().AlignCenter().Text(x =>
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

    public byte[] GenerateContractPdf(ContractData data)
    {
        // Similar implementation for contracts
        throw new NotImplementedException("Contract PDF generation coming soon");
    }

    void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            // Logo section
            row.ConstantItem(80).Height(50).Column(column =>
            {
                try
                {
                    // Load logo from S3 URL
                    using var httpClient = new HttpClient();
                    httpClient.Timeout = TimeSpan.FromSeconds(5);
                    var imageBytes = httpClient.GetByteArrayAsync("https://firmness-images.s3.us-east-2.amazonaws.com/logo.png").Result;
                    column.Item().Image(imageBytes).FitArea();
                }
                catch
                {
                    // Fallback: colored placeholder with text
                    column.Item().Background(Colors.Blue.Medium).AlignCenter().AlignMiddle()
                        .Text("LOGO").FontColor(Colors.White).FontSize(10).SemiBold();
                }
            });

            // Company info section
            row.RelativeItem().PaddingLeft(15).Column(column =>
            {
                column.Item().Text("FIRMNESS").FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);
                column.Item().Text("Construction Equipment Rental").FontSize(10).FontColor(Colors.Grey.Darken2);
                column.Item().Text("contact@firmness.com | +1 (555) 123-4567").FontSize(8).FontColor(Colors.Grey.Darken1);
            });
        });
    }

    void ComposeContent(IContainer container, ReceiptData data)
    {       
        container.PaddingVertical(20).Column(column =>
        {
            column.Spacing(10);

            column.Item().Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("RECEIPT").FontSize(18).SemiBold();
                    col.Item().Text($"Invoice #: {data.InvoiceNumber}").FontSize(10);
                    col.Item().Text($"Date: {data.Date:yyyy-MM-dd}").FontSize(10);
                });

                row.RelativeItem().Column(col =>
                {
                    col.Item().AlignRight().Text("Bill To:").SemiBold();
                    col.Item().AlignRight().Text(data.CustomerName);
                    col.Item().AlignRight().Text(data.CustomerEmail).FontSize(9);
                });
            });

            column.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

            // Items table
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("Description");
                    header.Cell().Element(CellStyle).AlignRight().Text("Qty");
                    header.Cell().Element(CellStyle).AlignRight().Text("Unit Price");
                    header.Cell().Element(CellStyle).AlignRight().Text("Total");

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });

                foreach (var item in data.Items)
                {
                    table.Cell().Element(CellStyle).Text(item.Description);
                    table.Cell().Element(CellStyle).AlignRight().Text(item.Quantity.ToString());
                    table.Cell().Element(CellStyle).AlignRight().Text($"${item.UnitPrice:N2}");
                    table.Cell().Element(CellStyle).AlignRight().Text($"${item.Total:N2}");

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                    }
                }
            });

            column.Item().PaddingTop(10).AlignRight().Row(row =>
            {
                row.ConstantItem(100).Text("TOTAL:").SemiBold().FontSize(14);
                row.ConstantItem(100).AlignRight().Text($"${data.TotalAmount:N2}").SemiBold().FontSize(14).FontColor(Colors.Green.Darken2);
            });

            column.Item().PaddingTop(20).Text($"Transaction ID: {data.TransactionId}").FontSize(9).FontColor(Colors.Grey.Darken1);
            column.Item().Text("Thank you for your business!").FontSize(10).Italic();
        });
    }
}
