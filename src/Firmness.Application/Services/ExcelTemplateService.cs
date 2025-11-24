using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Firmness.Application.Services;

public class ExcelTemplateService : IExcelTemplateService
{
    public byte[] GenerateTemplate()
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Template");

        // Headers
        worksheet.Cells[1, 1].Value = "SKU";
        worksheet.Cells[1, 2].Value = "Product";
        worksheet.Cells[1, 3].Value = "Description";
        worksheet.Cells[1, 4].Value = "Price";
        worksheet.Cells[1, 5].Value = "Cost";
        worksheet.Cells[1, 6].Value = "Stock";
        worksheet.Cells[1, 7].Value = "Minimum Stock";
        worksheet.Cells[1, 8].Value = "Barcode";
        worksheet.Cells[1, 9].Value = "Category";
        worksheet.Cells[1, 10].Value = "Image URL";
        worksheet.Cells[1, 11].Value = "Name";
        worksheet.Cells[1, 12].Value = "Last name";
        worksheet.Cells[1, 13].Value = "Email";
        worksheet.Cells[1, 14].Value = "Phone";
        worksheet.Cells[1, 15].Value = "Address";
        worksheet.Cells[1, 16].Value = "Document";
        worksheet.Cells[1, 17].Value = "Amount";

        // Header styling
        try
        {
            using (var range = worksheet.Cells[1, 1, 1, 17])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));
                range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.Border.BorderAround(ExcelBorderStyle.Medium);
            }
        }
        catch
        {
            // Fallback styling without colors if GDI+ is not available
            using (var range = worksheet.Cells[1, 1, 1, 17])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.Border.BorderAround(ExcelBorderStyle.Medium);
            }
        }

        // Add instructions
        worksheet.Cells["A3"].Value = "INSTRUCTIONS:";
        worksheet.Cells["A3"].Style.Font.Bold = true;
        worksheet.Cells["A3"].Style.Font.Size = 12;
        try
        {
            worksheet.Cells["A3"].Style.Font.Color.SetColor(System.Drawing.Color.DarkBlue);
        }
        catch { /* Ignore color errors */ }
        
        worksheet.Cells["A4"].Value = "• You can leave columns empty if you don't apply to that row";
        worksheet.Cells["A5"].Value = "• The system automatically identifies whether they are products, customers, or sales";
        worksheet.Cells["A6"].Value = "• Products are searched by SKU, customers by Email or Document";
        worksheet.Cells["A7"].Value = "• If it exists, it is updated; Otherwise, it is created";
        worksheet.Cells["A8"].Value = "• Required fields:";
        worksheet.Cells["A9"].Value = "- Products: SKU, Product, Price";
        worksheet.Cells["A10"].Value = "- Customers: First Name, Last Name, Email, ID";
        worksheet.Cells["A11"].Value = "- Sales: Product SKU, Customer Email, Quantity";
        worksheet.Cells["A12"].Value = "• The category must exactly match one of the available categories (see 'Categories' sheet)";
        worksheet.Cells["A13"].Value = "• Image URL: Direct link to the product image (optional)";
        using (var instrRange = worksheet.Cells["A4:A13"])
        {
            instrRange.Style.Font.Size = 10;
            instrRange.Style.WrapText = true;
        }

        // Auto-fit columns
        try { worksheet.Cells.AutoFitColumns(); } catch { /* Ignore GDI+ errors on Linux */ }
        
        // Adjust column widths
        worksheet.Column(2).Width = 30; // Producto
        worksheet.Column(3).Width = 40; // Descripcion
        worksheet.Column(9).Width = 20; // Categoria
        worksheet.Column(12).Width = 30; // Email
        worksheet.Column(14).Width = 35; // Direccion
        
        // Add Categories sheet
        AddCategoriesSheet(package);
        
        return package.GetAsByteArray();
    }

    public byte[] GenerateSampleData()
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Datos de Ejemplo");

        // Headers
        var headers = new[]
        {
            "SKU", "Product","Description", "Price", "Stock", "Cost", "Minimum Stock", "Barcode", "Category", "Image URL", "First Name", "Last Name","Email", "Phone", "Address"," ID Number", "Quantity"
        };

        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cells[1, i + 1].Value = headers[i];
        }

        // Header styling
        try
        {
            using (var range = worksheet.Cells[1, 1, 1, headers.Length])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));
                range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.Border.BorderAround(ExcelBorderStyle.Medium);
            }
        }
        catch
        {
            // Fallback styling without colors
            using (var range = worksheet.Cells[1, 1, 1, headers.Length])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.Border.BorderAround(ExcelBorderStyle.Medium);
            }
        }

        int row = 2;

        // Ejemplo 1: Cemento (usando categoría exacta del sistema)
        AddRow(worksheet, row++, 
            "CONST-001", "Cemento Portland Gris x 50kg", "Cemento tipo I para construcción general", 
            25.50m, 18.00m, 200, 50, "7701234567890", "Cement and Concrete (Cemento y Concreto)",
            "https://example.com/cemento.jpg");

        // Ejemplo 2: Producto + Cliente + Venta (herramientas eléctricas)
        AddRow(worksheet, row++, 
            "CONST-002", "Taladro Percutor DeWalt 850W", "Taladro eléctrico profesional con estuche y brocas", 
            185.99m, 120.00m, 35, 10, "7701234567891", "Power Tools (Herramientas Eléctricas)",
            "https://example.com/taladro.jpg",
            "Roberto", "Martínez", "roberto.martinez@construcciones.com", "+57 310 456 7890", 
            "Carrera 15 #85-32, Bogotá", "52123456", 2);

        // Ejemplo 3: Pisos y azulejos
        AddRow(worksheet, row++, 
            "CONST-003", "Cerámica Piso 45x45cm Beige", "Cerámica antideslizante para piso, caja x 2.03 m²", 
            42.99m, 28.50m, 150, 30, "7701234567892", "Floors and Tiles (Pisos y Azulejos)",
            "https://example.com/ceramica.jpg");

        // Ejemplo 4: Cliente nuevo (constructor)
        AddRow(worksheet, row++, 
            null, null, null, null, null, null, null, null, null, null,
            "Ana", "Gómez", "ana.gomez@email.com", "+57 315 678 9012", 
            "Calle 127 #45-67, Bogotá", "1098765432");

        // Ejemplo 5: Venta de cemento a cliente nuevo
        AddRow(worksheet, row++, 
            "CONST-001", null, null, null, null, null, null, null, null, null,
            "Ana", "Gómez", "ana.gomez@email.com", null, null, null, 50);

        // Ejemplo 6: Varilla y acero
        AddRow(worksheet, row++, 
            "CONST-004", "Varilla Corrugada 3/8\" x 6m", "Varilla de acero corrugado grado 60", 
            12.75m, 9.20m, 500, 100, "7701234567893", "Rebar and Steel (Varilla y Acero)",
            "https://example.com/varilla.jpg");

        // Ejemplo 7: Cliente + venta de varillas (empresa constructora)
        AddRow(worksheet, row++, 
            "CONST-004", null, null, null, null, null, null, null, null, null,
            "Construcciones", "Pérez S.A.S", "compras@construccionesperez.com", "+57 601 234 5678", 
            "Autopista Norte #145-30, Bogotá", "900123456", 100);

        // Ejemplo 8: Herramienta manual
        AddRow(worksheet, row++, 
            "CONST-005", "Nivel Láser Autonivelante", "Nivel láser de línea cruzada, alcance 30m", 
            145.00m, 95.00m, 25, 8, "7701234567894", "Hand Tools (Herramientas Manuales)",
            "https://example.com/nivel.jpg");

        // Ejemplo 9: Actualizar cliente existente (mismo email)
        AddRow(worksheet, row++, 
            null, null, null, null, null, null, null, null, null, null,
            "Roberto", "Martínez Vargas", "roberto.martinez@construcciones.com", "+57 310 999 8888", 
            "Carrera 15 #85-32 Oficina 201, Bogotá", "52123456");

        // Ejemplo 10: Material eléctrico
        AddRow(worksheet, row++, 
            "CONST-006", "Cable Eléctrico THW 12 AWG x 100m", "Cable de cobre calibre 12 para instalaciones eléctricas", 
            78.50m, 52.00m, 80, 20, "7701234567895", "Electrical (Electricidad)",
            "https://example.com/cable.jpg");

        // Ejemplo 11: Tubería PVC
        AddRow(worksheet, row++, 
            "CONST-007", "Tubería PVC 4\" x 3m Sanitaria", "Tubería PVC para desagües y alcantarillado", 
            18.90m, 12.50m, 120, 25, "7701234567896", "Pipe and PVC (Tubería y PVC)",
            "https://example.com/tuberia.jpg");

        // Ejemplo 12: Venta de cerámica a cliente existente
        AddRow(worksheet, row++, 
            "CONST-003", null, null, null, null, null, null, null, null, null,
            "Ana", "Gómez", "ana.gomez@email.com", null, null, null, 20);

        // Ejemplo 13: Pintura
        AddRow(worksheet, row++, 
            "CONST-008", "Pintura Vinílica Blanco 20L", "Pintura lavable para interiores y exteriores", 
            45.99m, 32.00m, 60, 15, "7701234567897", "Paints and Coatings (Pinturas y Recubrimientos)",
            "https://example.com/pintura.jpg");

        // Ejemplo 14: Renta de maquinaria - Montacargas
        AddRow(worksheet, row++, 
            "RENT-001", "Montacargas Eléctrico 2.5 Ton", "Montacargas eléctrico para renta diaria/mensual", 
            150.00m, 100.00m, 5, 2, "7701234567898", "Forklifts (Montacargas)",
            "https://example.com/montacargas.jpg");

        // Auto-fit columns
        try { worksheet.Cells.AutoFitColumns(); } catch { /* Ignore GDI+ errors on Linux */ }
        
        // Adjust specific column widths
        worksheet.Column(2).Width = 35;  // Producto
        worksheet.Column(3).Width = 50;  // Descripcion
        worksheet.Column(9).Width = 40;  // Categoria (más ancho para nombres completos)
        worksheet.Column(12).Width = 35; // Email
        worksheet.Column(14).Width = 40; // Direccion
        
        // Add note at bottom
        worksheet.Cells[row + 2, 1].Value = "NOTA: Este archivo contiene datos de ejemplo con categorías reales del sistema. Ver hoja 'Categorías Disponibles' para lista completa.";
        worksheet.Cells[row + 2, 1].Style.Font.Italic = true;
        try
        {
            worksheet.Cells[row + 2, 1].Style.Font.Color.SetColor(System.Drawing.Color.Gray);
        }
        catch { /* Ignore color errors */ }
        using (var range = worksheet.Cells[row + 2, 1, row + 2, headers.Length])
        {
            range.Merge = true;
        }
        
        // Add Categories sheet
        AddCategoriesSheet(package);
        
        return package.GetAsByteArray();
    }

    // Generate Excel with massive Vehicles data
    public byte[] GenerateVehiclesSampleData()
    {
        using var package = new ExcelPackage();
        
        // Vehicles Sheet
        var vehiclesSheet = package.Workbook.Worksheets.Add("Vehicles");
        
        // Headers
        var headers = new[]
        {
            "Brand", "Model", "Year", "License Plate", "Vehicle Type", "Serial Number",
            "Hourly Rate", "Daily Rate", "Weekly Rate", "Monthly Rate",
            "Current Hours", "Current Mileage", "Maintenance Hours Interval",
            "Specifications", "Image URL", "Notes", "Is Active"
        };

        for (int i = 0; i < headers.Length; i++)
        {
            vehiclesSheet.Cells[1, i + 1].Value = headers[i];
        }

        // Style headers
        try
        {
            using (var range = vehiclesSheet.Cells[1, 1, 1, headers.Length])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(255, 127, 80)); // Coral
                range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }
        }
        catch
        {
            // Fallback styling without colors
            using (var range = vehiclesSheet.Cells[1, 1, 1, headers.Length])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }
        }

        int row = 2;

        // Excavators (20 units)
        for (int i = 1; i <= 20; i++)
        {
            var brands = new[] { "Caterpillar", "Komatsu", "JCB", "Volvo", "Hitachi" };
            var models = new[] { "320GC", "PC210", "JS220", "EC210", "ZX210" };
            var brand = brands[(i - 1) % brands.Length];
            var model = models[(i - 1) % models.Length];
            
            AddVehicleRow(vehiclesSheet, row++, 
                brand, model, 2018 + (i % 6), $"EXC-{i:D3}", "Excavator", $"SN-EXC-{i:D6}",
                45.00m, 350.00m, 2100.00m, 7500.00m,
                1500 + (i * 50), 0, 500,
                $"{brand} {model} - 21 ton excavator with GPS and air conditioning",
                $"https://example.com/excavator_{i}.jpg",
                $"Excellent condition, recently serviced", true);
        }

        // Forklifts (25 units)
        for (int i = 1; i <= 25; i++)
        {
            var brands = new[] { "Toyota", "Hyster", "Yale", "Crown", "Nissan" };
            var models = new[] { "8FD25", "H2.5FT", "GDP25", "FC5200", "CPJ02" };
            var brand = brands[(i - 1) % brands.Length];
            var model = models[(i - 1) % models.Length];
            
            AddVehicleRow(vehiclesSheet, row++, 
                brand, model, 2019 + (i % 5), $"FLT-{i:D3}", "Forklift", $"SN-FLT-{i:D6}",
                25.00m, 180.00m, 1050.00m, 3800.00m,
                800 + (i * 30), 0, 250,
                $"{brand} {model} - 2.5 ton electric/LPG forklift",
                $"https://example.com/forklift_{i}.jpg",
                "Perfect for warehouses and distribution centers", true);
        }

        // Dump Trucks (15 units)
        for (int i = 1; i <= 15; i++)
        {
            var brands = new[] { "Volvo", "Mercedes-Benz", "Scania", "MAN", "Iveco" };
            var models = new[] { "FM440", "Actros 2644", "R440", "TGS 35.440", "Trakker 410" };
            var brand = brands[(i - 1) % brands.Length];
            var model = models[(i - 1) % models.Length];
            
            AddVehicleRow(vehiclesSheet, row++, 
                brand, model, 2017 + (i % 7), $"DMT-{i:D3}", "DumpTruck", $"SN-DMT-{i:D6}",
                55.00m, 420.00m, 2520.00m, 9200.00m,
                0, 45000 + (i * 5000), 500,
                $"{brand} {model} - 14 cubic meter dump truck",
                $"https://example.com/dumptruck_{i}.jpg",
                "Great for material transport", true);
        }

        // Cranes (10 units)
        for (int i = 1; i <= 10; i++)
        {
            var brands = new[] { "Liebherr", "Terex", "Grove", "Tadano", "Manitowoc" };
            var models = new[] { "LTM1100", "AC100", "GMK5100", "ATF100", "MLC100" };
            var brand = brands[(i - 1) % brands.Length];
            var model = models[(i - 1) % models.Length];
            
            AddVehicleRow(vehiclesSheet, row++, 
                brand, model, 2016 + (i % 8), $"CRN-{i:D3}", "Crane", $"SN-CRN-{i:D6}",
                80.00m, 650.00m, 3900.00m, 14500.00m,
                0, 25000 + (i * 3000), 300,
                $"{brand} {model} - 100 ton mobile crane",
                $"https://example.com/crane_{i}.jpg",
                "Certified operator included", true);
        }

        // Backhoes (18 units)
        for (int i = 1; i <= 18; i++)
        {
            var brands = new[] { "Caterpillar", "JCB", "Case", "John Deere", "Terex" };
            var models = new[] { "420F", "3CX", "580N", "310L", "TX760" };
            var brand = brands[(i - 1) % brands.Length];
            var model = models[(i - 1) % models.Length];
            
            AddVehicleRow(vehiclesSheet, row++, 
                brand, model, 2018 + (i % 6), $"BCK-{i:D3}", "Backhoe", $"SN-BCK-{i:D6}",
                35.00m, 280.00m, 1680.00m, 6000.00m,
                1200 + (i * 40), 0, 400,
                $"{brand} {model} - Loader backhoe with extendable arm",
                $"https://example.com/backhoe_{i}.jpg",
                "Versatile for excavation and loading", true);
        }

        // Front Loaders (12 units)
        for (int i = 1; i <= 12; i++)
        {
            var brands = new[] { "Caterpillar", "Komatsu", "Volvo", "JCB", "Case" };
            var models = new[] { "950GC", "WA200", "L90", "456", "621F" };
            var brand = brands[(i - 1) % brands.Length];
            var model = models[(i - 1) % models.Length];
            
            AddVehicleRow(vehiclesSheet, row++, 
                brand, model, 2019 + (i % 5), $"LDR-{i:D3}", "FrontLoader", $"SN-LDR-{i:D6}",
                40.00m, 320.00m, 1920.00m, 7000.00m,
                900 + (i * 35), 0, 350,
                $"{brand} {model} - Wheel loader 3 cubic yards",
                $"https://example.com/loader_{i}.jpg",
                "Ideal for material handling", true);
        }

        // Auto-fit columns
        try { vehiclesSheet.Cells.AutoFitColumns(); } catch { /* Ignore GDI+ errors on Linux */ }
        vehiclesSheet.Column(1).Width = 15;  // Brand
        vehiclesSheet.Column(2).Width = 15;  // Model
        vehiclesSheet.Column(14).Width = 50; // Specifications
        vehiclesSheet.Column(16).Width = 35; // Notes

        // Add Instructions Sheet
        AddVehiclesInstructionsSheet(package);
        
        // Add Vehicle Types Reference
        AddVehicleTypesSheet(package);
        
        return package.GetAsByteArray();
    }

    private void AddVehicleRow(ExcelWorksheet sheet, int row, 
        string brand, string model, int year, string licensePlate, string vehicleType, string serialNumber,
        decimal hourlyRate, decimal dailyRate, decimal weeklyRate, decimal monthlyRate,
        decimal currentHours, decimal currentMileage, decimal maintenanceInterval,
        string specifications, string imageUrl, string notes, bool isActive)
    {
        sheet.Cells[row, 1].Value = brand;
        sheet.Cells[row, 2].Value = model;
        sheet.Cells[row, 3].Value = year;
        sheet.Cells[row, 4].Value = licensePlate;
        sheet.Cells[row, 5].Value = vehicleType;
        sheet.Cells[row, 6].Value = serialNumber;
        sheet.Cells[row, 7].Value = hourlyRate;
        sheet.Cells[row, 8].Value = dailyRate;
        sheet.Cells[row, 9].Value = weeklyRate;
        sheet.Cells[row, 10].Value = monthlyRate;
        sheet.Cells[row, 11].Value = currentHours;
        sheet.Cells[row, 12].Value = currentMileage;
        sheet.Cells[row, 13].Value = maintenanceInterval;
        sheet.Cells[row, 14].Value = specifications;
        sheet.Cells[row, 15].Value = imageUrl;
        sheet.Cells[row, 16].Value = notes;
        sheet.Cells[row, 17].Value = isActive ? "Yes" : "No";

        // Format currency columns
        sheet.Cells[row, 7].Style.Numberformat.Format = "$#,##0.00";
        sheet.Cells[row, 8].Style.Numberformat.Format = "$#,##0.00";
        sheet.Cells[row, 9].Style.Numberformat.Format = "$#,##0.00";
        sheet.Cells[row, 10].Style.Numberformat.Format = "$#,##0.00";
    }

    private void AddVehiclesInstructionsSheet(ExcelPackage package)
    {
        var instrSheet = package.Workbook.Worksheets.Add("Instructions");
        
        instrSheet.Cells["A1"].Value = "VEHICLES BULK IMPORT - INSTRUCTIONS";
        instrSheet.Cells["A1"].Style.Font.Bold = true;
        instrSheet.Cells["A1"].Style.Font.Size = 16;
        try
        {
            instrSheet.Cells["A1"].Style.Font.Color.SetColor(System.Drawing.Color.DarkBlue);
        }
        catch { /* Ignore color errors */ }
        
        int row = 3;
        instrSheet.Cells[row++, 1].Value = "HOW TO USE THIS FILE:";
        instrSheet.Cells[row - 1, 1].Style.Font.Bold = true;
        
        instrSheet.Cells[row++, 1].Value = "1. Review the 'Vehicles' sheet with 100 sample records";
        instrSheet.Cells[row++, 1].Value = "2. Modify the data as needed or add new records";
        instrSheet.Cells[row++, 1].Value = "3. Save the file and upload it to the Bulk Import section";
        instrSheet.Cells[row++, 1].Value = "4. The system will create/update vehicles automatically";
        
        row++;
        instrSheet.Cells[row++, 1].Value = "REQUIRED FIELDS:";
        instrSheet.Cells[row - 1, 1].Style.Font.Bold = true;
        instrSheet.Cells[row++, 1].Value = "• Brand (e.g., Caterpillar, Toyota, Volvo)";
        instrSheet.Cells[row++, 1].Value = "• Model (e.g., 320GC, 8FD25, FM440)";
        instrSheet.Cells[row++, 1].Value = "• Year (e.g., 2020)";
        instrSheet.Cells[row++, 1].Value = "• License Plate (unique identifier)";
        instrSheet.Cells[row++, 1].Value = "• Vehicle Type (see 'Vehicle Types' sheet)";
        instrSheet.Cells[row++, 1].Value = "• At least one rate (Hourly, Daily, Weekly, or Monthly)";
        
        row++;
        instrSheet.Cells[row++, 1].Value = "VEHICLE TYPES AVAILABLE:";
        instrSheet.Cells[row - 1, 1].Style.Font.Bold = true;
        instrSheet.Cells[row++, 1].Value = "Excavator, Forklift, DumpTruck, Crane, Backhoe, FrontLoader,";
        instrSheet.Cells[row++, 1].Value = "Compactor, ConcreteMixer, Generator, AirCompressor, Scaffolding,";
        instrSheet.Cells[row++, 1].Value = "TelephoneHoist, BoomLift, ScisorLift, Other";
        
        row++;
        instrSheet.Cells[row++, 1].Value = "TIPS:";
        instrSheet.Cells[row - 1, 1].Style.Font.Bold = true;
        instrSheet.Cells[row++, 1].Value = "• License Plate must be unique - duplicates will update existing vehicles";
        instrSheet.Cells[row++, 1].Value = "• Rates are in USD - use format: 150.00";
        instrSheet.Cells[row++, 1].Value = "• Is Active: 'Yes' or 'No' (default: Yes)";
        instrSheet.Cells[row++, 1].Value = "• Leave fields empty if not applicable";
        
        try { instrSheet.Cells.AutoFitColumns(); } catch { /* Ignore GDI+ errors on Linux */ }
        instrSheet.Column(1).Width = 70;
    }

    private void AddVehicleTypesSheet(ExcelPackage package)
    {
        var typesSheet = package.Workbook.Worksheets.Add("Vehicle Types");
        
        typesSheet.Cells["A1"].Value = "VEHICLE TYPES REFERENCE";
        typesSheet.Cells["A1"].Style.Font.Bold = true;
        typesSheet.Cells["A1"].Style.Font.Size = 14;
        
        typesSheet.Cells["A3"].Value = "Type Code";
        typesSheet.Cells["B3"].Value = "Description";
        typesSheet.Cells["C3"].Value = "Typical Use";
        
        try
        {
            using (var range = typesSheet.Cells["A3:C3"])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }
        }
        catch
        {
            // Fallback styling without colors
            using (var range = typesSheet.Cells["A3:C3"])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            }
        }
        
        int row = 4;
        var types = new[]
        {
            ("Excavator", "Track or wheel excavators", "Excavation, demolition, earthmoving"),
            ("Forklift", "Electric or combustion forklifts", "Material handling, warehouses"),
            ("DumpTruck", "Dump trucks various capacities", "Material transport"),
            ("Crane", "Mobile or tower cranes", "Heavy lifting, construction"),
            ("Backhoe", "Loader backhoes", "Excavation and loading"),
            ("FrontLoader", "Wheel loaders", "Material loading and transport"),
            ("Compactor", "Vibratory or static compactors", "Soil and asphalt compaction"),
            ("ConcreteMixer", "Concrete mixers", "Concrete preparation"),
            ("Generator", "Electric generators", "Temporary power supply"),
            ("AirCompressor", "Portable compressors", "Pneumatic tools"),
            ("Scaffolding", "Modular scaffolding", "Work at height"),
            ("TelephoneHoist", "Material hoists", "Vertical material transport"),
            ("BoomLift", "Articulated or telescopic lifts", "Elevated work"),
            ("ScisorLift", "Scissor lifts", "Stable elevated work"),
            ("Other", "Other equipment", "Various uses")
        };
        
        foreach (var (type, desc, use) in types)
        {
            typesSheet.Cells[row, 1].Value = type;
            typesSheet.Cells[row, 2].Value = desc;
            typesSheet.Cells[row, 3].Value = use;
            row++;
        }
        
        try { typesSheet.Cells.AutoFitColumns(); } catch { /* Ignore GDI+ errors on Linux */ }
    }

    private void AddCategoriesSheet(ExcelPackage package)
    {
        var catSheet = package.Workbook.Worksheets.Add("Categorías Disponibles");
        
        // Title
        catSheet.Cells["A1"].Value = "CATEGORÍAS DISPONIBLES EN EL SISTEMA";
        catSheet.Cells["A1"].Style.Font.Bold = true;
        catSheet.Cells["A1"].Style.Font.Size = 14;
        try
        {
            catSheet.Cells["A1"].Style.Font.Color.SetColor(System.Drawing.Color.DarkBlue);
        }
        catch { /* Ignore color errors */ }
        using (var range = catSheet.Cells["A1:B1"])
        {
            range.Merge = true;
        }
        
        // Headers
        catSheet.Cells["A3"].Value = "Nombre de Categoría (usar exactamente como aparece)";
        catSheet.Cells["B3"].Value = "Descripción";
        
        try
        {
            using (var range = catSheet.Cells["A3:B3"])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));
                range.Style.Font.Color.SetColor(System.Drawing.Color.White);
            }
        }
        catch
        {
            // Fallback styling without colors
            using (var range = catSheet.Cells["A3:B3"])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            }
        }
        
        int row = 4;
        
        // Construction Materials Section
        catSheet.Cells[row, 1].Value = "MATERIALES DE CONSTRUCCIÓN";
        catSheet.Cells[row, 1].Style.Font.Bold = true;
        catSheet.Cells[row, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
        try
        {
            catSheet.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        }
        catch { /* Ignore color errors */ }
        using (var range = catSheet.Cells[row, 1, row, 2])
        {
            range.Merge = true;
        }
        row++;
        
        var constructionCategories = new[]
        {
            ("Cement and Concrete (Cemento y Concreto)", "Cement, ready-mix concrete, mortar and related products"),
            ("Aggregates (Agregados)", "Sand, gravel, crushed stone and fill materials"),
            ("Block and Brick (Block y Ladrillo)", "Concrete blocks, bricks, tiles and masonry materials"),
            ("Rebar and Steel (Varilla y Acero)", "Corrugated rebar, wire rod, welded mesh and metal structures"),
            ("Wood and Boards (Madera y Tableros)", "Construction lumber, plywood, MDF, OSB and various boards"),
            ("Pipe and PVC (Tubería y PVC)", "Hydraulic, sanitary, electrical piping in PVC, copper and other materials"),
            ("Hardware and Fittings (Herrajes y Ferretería)", "Nails, screws, hinges, locks and metal accessories"),
            ("Paints and Coatings (Pinturas y Recubrimientos)", "Vinyl paints, enamels, waterproofing and sealants"),
            ("Floors and Tiles (Pisos y Azulejos)", "Ceramic, porcelain, tile, marble and floor and wall coverings"),
            ("Doors and Windows (Puertas y Ventanas)", "Wooden, metal, aluminum doors, windows and frames"),
            ("Electrical (Electricidad)", "Electrical wire, outlets, switches, lighting and accessories"),
            ("Plumbing and Bathrooms (Plomería y Baños)", "Faucets, showers, toilets, sinks, bathtubs and sanitary accessories"),
            ("Waterproofing (Impermeabilizantes)", "Membranes, asphalt products, sealants and waterproofing systems"),
            ("Insulation (Aislantes)", "Thermal insulation, acoustic, fiberglass and foams"),
            ("Adhesives and Glues (Adhesivos y Pegamentos)", "Construction adhesives, ceramic glue, silicones"),
            ("Hand Tools (Herramientas Manuales)", "Hammers, shovels, wheelbarrows, levels and masonry tools"),
            ("Power Tools (Herramientas Eléctricas)", "Drills, saws, grinders, rotary hammers and power equipment"),
            ("Industrial Safety (Seguridad Industrial)", "Helmets, gloves, harnesses, boots and personal protective equipment"),
            ("Geotextiles and Drainage (Geotextiles y Drenaje)", "Geotextile meshes, drainage piping and filtration systems"),
            ("Aluminum Windows (Cancelería de Aluminio)", "Aluminum profiles, glass, hardware and window accessories"),
        };
        
        foreach (var (name, desc) in constructionCategories)
        {
            catSheet.Cells[row, 1].Value = name;
            catSheet.Cells[row, 2].Value = desc;
            row++;
        }
        
        // Rental Section
        row++;
        catSheet.Cells[row, 1].Value = "RENTA DE VEHÍCULOS Y MAQUINARIA INDUSTRIAL";
        catSheet.Cells[row, 1].Style.Font.Bold = true;
        catSheet.Cells[row, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
        try
        {
            catSheet.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        }
        catch { /* Ignore color errors */ }
        using (var range = catSheet.Cells[row, 1, row, 2])
        {
            range.Merge = true;
        }
        row++;
        
        var rentalCategories = new[]
        {
            ("Forklifts (Montacargas)", "Electric forklifts, combustion, reach trucks and stackers"),
            ("Excavators (Excavadoras)", "Track excavators, wheeled excavators, mini excavators"),
            ("Backhoes (Retroexcavadoras)", "Backhoes, front loaders with backhoe arm"),
            ("Front Loaders (Cargadores Frontales)", "Wheel loaders, mini loaders, skid steer loaders"),
            ("Compactors (Compactadoras)", "Compactor rollers, vibratory compactors and rammers"),
            ("Cranes and Booms (Grúas y Plumas)", "Telescopic cranes, tower cranes, hydraulic booms"),
            ("Dump Trucks (Camiones de Volteo)", "Dump trucks 3.5, 7, 14 tons for material transport"),
            ("Concrete Mixers (Revolvedoras de Concreto)", "Portable mixers, concrete and mortar mixers"),
            ("Electric Generators (Generadores Eléctricos)", "Diesel generators, gasoline, portable and industrial power plants"),
            ("Air Compressors (Compresores de Aire)", "Portable compressors, industrial, screw and piston"),
            ("Water Pumps (Bombas de Agua)", "Submersible pumps, centrifugal, drainage and sludge pumps"),
            ("Scaffolding and Lifts (Andamios y Elevadores)", "Tubular scaffolding, personnel lifts, scissor platforms"),
            ("Cutting Equipment (Equipos de Corte)", "Concrete cutters, asphalt, hand and disc saws"),
            ("Demolition Hammers (Martillos Demoledores)", "Pneumatic hammers, hydraulic, electric breakers"),
            ("Concrete Vibrators (Vibradores de Concreto)", "Immersion vibrators, vibrating screed, vibrating tables"),
            ("Light Towers (Torres de Iluminación)", "Construction light towers, portable floodlights"),
            ("Welding Equipment (Equipos de Soldadura)", "Welding machines, welding plants, cutting equipment"),
            ("Motor Graders (Motoconformadoras)", "Motor graders, leveling and land forming equipment"),
            ("Tractors and Bulldozers (Tractores y Bulldozers)", "Track tractors, bulldozers, earthmoving equipment"),
            ("Aerial Platforms (Plataformas Elevadoras)", "Scissor platforms, articulated, telescopic for work at height"),
        };
        
        foreach (var (name, desc) in rentalCategories)
        {
            catSheet.Cells[row, 1].Value = name;
            catSheet.Cells[row, 2].Value = desc;
            row++;
        }
        
        // Auto-fit and adjust columns
        try { catSheet.Cells.AutoFitColumns(); } catch { /* Ignore GDI+ errors on Linux */ }
        catSheet.Column(1).Width = 50;
        catSheet.Column(2).Width = 70;
        
        // Add note
        row += 2;
        catSheet.Cells[row, 1].Value = "IMPORTANTE: Al importar productos, el nombre de la categoría debe coincidir EXACTAMENTE con uno de los nombres de esta lista.";
        catSheet.Cells[row, 1].Style.Font.Bold = true;
        try
        {
            catSheet.Cells[row, 1].Style.Font.Color.SetColor(System.Drawing.Color.Red);
        }
        catch { /* Ignore color errors */ }
        using (var range = catSheet.Cells[row, 1, row, 2])
        {
            range.Merge = true;
            range.Style.WrapText = true;
        }
    }

    private void AddRow(ExcelWorksheet ws, int row,
        string? sku = null, string? producto = null, string? descripcion = null,
        decimal? precio = null, decimal? costo = null, int? stock = null, int? stockMin = null,
        string? barcode = null, string? categoria = null, string? imageUrl = null,
        string? nombre = null, string? apellido = null, string? email = null,
        string? telefono = null, string? direccion = null, string? documento = null,
        int? cantidad = null)
    {
        int col = 1;
        ws.Cells[row, col++].Value = sku;
        ws.Cells[row, col++].Value = producto;
        ws.Cells[row, col++].Value = descripcion;
        ws.Cells[row, col++].Value = precio;
        ws.Cells[row, col++].Value = costo;
        ws.Cells[row, col++].Value = stock;
        ws.Cells[row, col++].Value = stockMin;
        ws.Cells[row, col++].Value = barcode;
        ws.Cells[row, col++].Value = categoria;
        ws.Cells[row, col++].Value = imageUrl; // New column
        ws.Cells[row, col++].Value = nombre;
        ws.Cells[row, col++].Value = apellido;
        ws.Cells[row, col++].Value = email;
        ws.Cells[row, col++].Value = telefono;
        ws.Cells[row, col++].Value = direccion;
        ws.Cells[row, col++].Value = documento;
        ws.Cells[row, col++].Value = cantidad;
    }
}
