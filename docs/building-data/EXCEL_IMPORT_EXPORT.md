# 📊 Excel Import/Export Features

> [⬅️ Back to Main README](../../README.md) | [📚 Documentation Hub](../README.md)

Manage bulk data operations efficiently using **EPPlus (v5.2.5)**. This module handles mass creation of customers and vehicles, as well as reporting.

## 🚀 Features Overview

| Feature                 | Description                                                   | Endpoint                          |
| ----------------------- | ------------------------------------------------------------- | --------------------------------- |
| **👥 Import Customers** | Create users in bulk from Excel. Validates emails & docs.     | `POST /api/bulk-import/customers` |
| **🚜 Import Vehicles**  | Add fleet inventory in bulk. Auto-sets status to 'Available'. | `POST /api/bulk-import/vehicles`  |
| **📋 Export Rentals**   | Download full rental history report.                          | `GET /api/export/rentals`         |

---

## 👨‍💻 Developer Integration Guide

<details>
<summary><strong>📦 1. Dependencies & Configuration (Click to Expand)</strong></summary>

### NuGet Package

Ensure **EPPlus 5.2.5** is installed in `Firmness.Infrastructure`.

```xml
<PackageReference Include="EPPlus" Version="5.2.5" />
```

### License Configuration (Mandatory)

EPPlus 5+ requires explicit license context. Add this to `Program.cs`:

```csharp
// Program.cs or Startup.cs
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
```

</details>

<details>
<summary><strong>🛠️ 2. Service Implementation (Click to Expand)</strong></summary>

**File:** `Firmness.Application/Services/BulkImportService.cs`

```csharp
using OfficeOpenXml;

public async Task<Result<ImportSummary>> ImportCustomersAsync(Stream fileStream)
{
    // Ensure context is set
    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

    using var package = new ExcelPackage(fileStream);
    var worksheet = package.Workbook.Worksheets[0];
    var rowCount = worksheet.Dimension.Rows;

    var successCount = 0;
    var errors = new List<string>();

    // Iterate rows (Skip header: start at 2)
    for (int row = 2; row <= rowCount; row++)
    {
        try
        {
            var email = worksheet.Cells[row, 3].Value?.ToString();
            // ... Validation & Creation Logic ...
            successCount++;
        }
        catch (Exception ex)
        {
            errors.Add($"Row {row}: {ex.Message}");
        }
    }

    return Result<ImportSummary>.Success(new ImportSummary(successCount, errors));
}
```

</details>

<details>
<summary><strong>📤 3. Export Implementation (Click to Expand)</strong></summary>

**File:** `Firmness.Application/Services/ExportService.cs`

```csharp
public async Task<byte[]> ExportRentalsAsync()
{
    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

    using var package = new ExcelPackage();
    var ws = package.Workbook.Worksheets.Add("Rentals");

    // Load data directly from collection
    var rentals = await _rentalRepository.GetAllAsync();
    ws.Cells["A1"].LoadFromCollection(rentals, true); // true = Print Headers

    return package.GetAsByteArray();
}
```

</details>

---

## 📝 Standard Templates

Users must upload files matching these structures:

### Customers Template

| FirstName | LastName | Email         | Document  | Phone    |
| --------- | -------- | ------------- | --------- | -------- |
| John      | Doe      | john@test.com | 123456789 | 555-0101 |

### Vehicles Template

| Brand | Model | Year | LicensePlate | Rate   |
| ----- | ----- | ---- | ------------ | ------ |
| CAT   | 320D  | 2022 | CAT-001      | 150.00 |

---

<div align="center">
  <a href="../../README.md">⬅️ Back to Main README</a> | 
  <a href="../README.md">📚 Documentation Hub</a> | 
  <a href="PDF_GENERATION.md">📝 PDF Generation</a>
</div>
