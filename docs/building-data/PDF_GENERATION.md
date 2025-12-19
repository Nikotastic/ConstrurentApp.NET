# 📄 PDF Generation Features

Generate professional, legally binding documents using **QuestPDF**. This module handles dynamic creation of rental contracts and invoices.

## 🚀 Features Overview

| Document               | Description                                                       | Trigger             |
| ---------------------- | ----------------------------------------------------------------- | ------------------- |
| **📝 Rental Contract** | Legal agreement with terms, vehicle specs, and liability clauses. | Rental Confirmation |
| **🧾 Invoice**         | Financial breakdown including taxes, discounts, and totals.       | Payment / Checkout  |

---

## 👨‍💻 Developer Integration Guide

<details>
<summary><strong>📦 1. Dependencies & Configuration (Click to Expand)</strong></summary>

### NuGet Packages

The project uses **QuestPDF** (Fluent API) and **iText7** (Legacy support).

```xml
<!-- Firmness.Web.csproj -->
<PackageReference Include="QuestPDF" Version="2024.*" />
<PackageReference Include="itext7" Version="9.3.0" />
```

### License Configuration

QuestPDF requires a license setting (Community License is free).

```csharp
// Program.cs
QuestPDF.Settings.License = LicenseType.Community;
```

</details>

<details>
<summary><strong>🛠️ 2. QuestPDF Implementation (Recommended) (Click to Expand)</strong></summary>

We use the **Fluent API** to design documents programmatically.

**File:** `Firmness.Infrastructure/Documents/RentalContractDocument.cs`

```csharp
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

public class RentalContractDocument : IDocument
{
    public RentalContractDto Model { get; }

    public RentalContractDocument(RentalContractDto model)
    {
        Model = model;
    }

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(50);
            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeContent);
            page.Footer().AlignCenter().Text(x =>
            {
                x.CurrentPageNumber();
                x.Span(" / ");
                x.TotalPages();
            });
        });
    }

    void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(col =>
            {
                col.Item().Text($"Contract #{Model.ContractNumber}").FontSize(20).SemiBold();
                col.Item().Text(DateTime.Now.ToString("d"));
            });

            // Add Logo
            // row.ConstantItem(100).Image("logo.png");
        });
    }

    void ComposeContent(IContainer container)
    {
        container.PaddingVertical(20).Column(col =>
        {
            col.Item().Text($"Customer: {Model.CustomerName}");
            col.Item().Text($"Vehicle: {Model.VehicleName}");

            // Add Table
            col.Item().PaddingTop(10).Table(table =>
            {
                table.ColumnsDefinition(c => { c.RelativeColumn(); c.RelativeColumn(); });
                table.Header(h => { h.Cell().Text("Item"); h.Cell().AlignRight().Text("Cost"); });

                table.Cell().Text("Rental Rate");
                table.Cell().AlignRight().Text($"{Model.TotalAmount:C}");
            });
        });
    }
}
```

</details>

<details>
<summary><strong>🚀 3. Usage in Controller (Click to Expand)</strong></summary>

**File:** `Firmness.Api/Controllers/DocumentsController.cs`

```csharp
[HttpGet("rentals/{id}/contract")]
public async Task<IActionResult> GetContract(Guid id)
{
    var rental = await _rentalService.GetByIdAsync(id);

    // Generate PDF bytes
    var document = new RentalContractDocument(rental);
    var pdfBytes = document.GeneratePdf();

    return File(pdfBytes, "application/pdf", $"Contract_{rental.ContractNumber}.pdf");
}
```

</details>

---

## 💡 Best Practices

- **Fonts**: QuestPDF uses system fonts. Ensure fonts (like Arial or Roboto) are installed in your Docker container if deployment looks different from local.
- **Images**: Use embedded resources or Base64 strings for logos to avoid file path issues in production.
