using Firmness.Application.Interfaces;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace Firmness.Application.Services;


// Service for generating Excel templates for bulk data import
public class ExcelTemplateService : IExcelTemplateService
{
    public byte[] GenerateProductTemplate()
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Products - Productos");

        // Configure column widths
        worksheet.Column(1).Width = 15;  // SKU / Código
        worksheet.Column(2).Width = 35;  // Name / Nombre
        worksheet.Column(3).Width = 45;  // Description / Descripción
        worksheet.Column(4).Width = 12;  // Price / Precio
        worksheet.Column(5).Width = 10;  // Stock / Existencia
        worksheet.Column(6).Width = 50;  // Image URL / URL Imagen
        worksheet.Column(7).Width = 20;  // Category / Categoría

        // Header row styling
        using (var range = worksheet.Cells["A1:G1"])
        {
            range.Style.Font.Bold = true;
            range.Style.Font.Size = 12;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(68, 114, 196)); // Blue
            range.Style.Font.Color.SetColor(Color.White);
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Row(1).Height = 30;
        }

        // Bilingual Headers (English / Spanish)
        worksheet.Cells["A1"].Value = "SKU / Código";
        worksheet.Cells["B1"].Value = "Name * / Nombre *";
        worksheet.Cells["C1"].Value = "Description / Descripción";
        worksheet.Cells["D1"].Value = "Price * / Precio *";
        worksheet.Cells["E1"].Value = "Stock / Existencia";
        worksheet.Cells["F1"].Value = "Image URL / URL Imagen";
        worksheet.Cells["G1"].Value = "Category / Categoría";

        // Add example rows
        worksheet.Cells["A2"].Value = "PROD-001";
        worksheet.Cells["B2"].Value = "Cement Portland Type I / Cemento Portland Tipo I";
        worksheet.Cells["C2"].Value = "High quality cement for construction / Cemento de alta calidad para construcción";
        worksheet.Cells["D2"].Value = 25.50;
        worksheet.Cells["E2"].Value = 100;
        worksheet.Cells["F2"].Value = "https://example.com/cement.jpg";
        worksheet.Cells["G2"].Value = "Construction Materials / Materiales de Construcción";

        worksheet.Cells["A3"].Value = "PROD-002";
        worksheet.Cells["B3"].Value = "Steel Rebar 12mm / Varilla de Acero 12mm";
        worksheet.Cells["C3"].Value = "Reinforcement steel bar / Varilla de refuerzo de acero";
        worksheet.Cells["D3"].Value = 8.75;
        worksheet.Cells["E3"].Value = 500;
        worksheet.Cells["F3"].Value = "";
        worksheet.Cells["G3"].Value = "Steel / Acero";

        // Style example rows
        using (var range = worksheet.Cells["A2:G3"])
        {
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(217, 225, 242)); // Light blue
        }

        // Add bilingual instructions
        worksheet.Cells["A5"].Value = "INSTRUCTIONS / INSTRUCCIONES:";
        worksheet.Cells["A5"].Style.Font.Bold = true;
        worksheet.Cells["A5"].Style.Font.Size = 11;
        
        worksheet.Cells["A6"].Value = "• Fields marked with * are required / Los campos marcados con * son obligatorios";
        worksheet.Cells["A7"].Value = "• SKU: Unique product code (auto-generated if empty) / Código único del producto (se genera automáticamente si está vacío)";
        worksheet.Cells["A8"].Value = "• Price: Must be greater than 0 / El precio debe ser mayor que 0";
        worksheet.Cells["A9"].Value = "• Stock: Default is 0 if empty / Por defecto es 0 si está vacío";
        worksheet.Cells["A10"].Value = "• Column names can be in English or Spanish / Los nombres de columnas pueden estar en inglés o español";
        worksheet.Cells["A11"].Value = "• Accepted column names: SKU/Codigo, Name/Nombre/Producto, Price/Precio, Stock/Existencia, etc.";
        worksheet.Cells["A12"].Value = "• Delete the example rows before importing / Elimine las filas de ejemplo antes de importar";
        
        using (var range = worksheet.Cells["A6:A12"])
        {
            range.Style.Font.Italic = true;
            range.Style.Font.Color.SetColor(Color.FromArgb(128, 128, 128));
        }

        // Freeze header row
        worksheet.View.FreezePanes(2, 1);

        return package.GetAsByteArray();
    }

    public byte[] GenerateVehicleTemplate()
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Vehicles - Vehículos");

        // Configure column widths
        worksheet.Column(1).Width = 18;  // Brand / Marca
        worksheet.Column(2).Width = 22;  // Model / Modelo
        worksheet.Column(3).Width = 8;   // Year / Año
        worksheet.Column(4).Width = 18;  // License Plate / Placa
        worksheet.Column(5).Width = 22;  // Vehicle Type / Tipo
        worksheet.Column(6).Width = 14;  // Daily Rate / Tarifa Diaria
        worksheet.Column(7).Width = 14;  // Weekly Rate / Tarifa Semanal
        worksheet.Column(8).Width = 14;  // Monthly Rate / Tarifa Mensual
        worksheet.Column(9).Width = 18;  // Serial Number / Número de Serie
        worksheet.Column(10).Width = 50; // Image URL / URL Imagen
        worksheet.Column(11).Width = 35; // Notes / Notas

        // Header row styling
        using (var range = worksheet.Cells["A1:K1"])
        {
            range.Style.Font.Bold = true;
            range.Style.Font.Size = 11;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(112, 173, 71)); // Green
            range.Style.Font.Color.SetColor(Color.White);
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Row(1).Height = 35;
            range.Style.WrapText = true;
        }

        // Bilingual Headers
        worksheet.Cells["A1"].Value = "Brand * / Marca *";
        worksheet.Cells["B1"].Value = "Model * / Modelo *";
        worksheet.Cells["C1"].Value = "Year * / Año *";
        worksheet.Cells["D1"].Value = "License Plate * / Placa *";
        worksheet.Cells["E1"].Value = "Vehicle Type * / Tipo *";
        worksheet.Cells["F1"].Value = "Daily Rate * / Tarifa Diaria *";
        worksheet.Cells["G1"].Value = "Weekly Rate / Tarifa Semanal";
        worksheet.Cells["H1"].Value = "Monthly Rate / Tarifa Mensual";
        worksheet.Cells["I1"].Value = "Serial Number / Número de Serie";
        worksheet.Cells["J1"].Value = "Image URL / URL Imagen";
        worksheet.Cells["K1"].Value = "Notes / Notas";

        // Add example rows
        worksheet.Cells["A2"].Value = "Caterpillar";
        worksheet.Cells["B2"].Value = "320D";
        worksheet.Cells["C2"].Value = 2020;
        worksheet.Cells["D2"].Value = "CAT-001";
        worksheet.Cells["E2"].Value = "Excavator";
        worksheet.Cells["F2"].Value = 350.00;
        worksheet.Cells["G2"].Value = 2100.00;
        worksheet.Cells["H2"].Value = 8000.00;
        worksheet.Cells["I2"].Value = "CAT320D2020001";
        worksheet.Cells["J2"].Value = "https://example.com/excavator.jpg";
        worksheet.Cells["K2"].Value = "20-ton hydraulic excavator / Excavadora hidráulica de 20 toneladas";

        worksheet.Cells["A3"].Value = "Case";
        worksheet.Cells["B3"].Value = "580N";
        worksheet.Cells["C3"].Value = 2019;
        worksheet.Cells["D3"].Value = "BCK-013";
        worksheet.Cells["E3"].Value = "Backhoe";
        worksheet.Cells["F3"].Value = 280.00;
        worksheet.Cells["G3"].Value = 1680.00;
        worksheet.Cells["H3"].Value = 6400.00;
        worksheet.Cells["I3"].Value = "";
        worksheet.Cells["J3"].Value = "";
        worksheet.Cells["K3"].Value = "Backhoe loader with 4WD / Retroexcavadora con tracción 4x4";

        // Style example rows
        using (var range = worksheet.Cells["A2:K3"])
        {
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(226, 239, 218)); // Light green
        }

        // Add bilingual notes
        worksheet.Cells["A5"].Value = "INSTRUCTIONS / INSTRUCCIONES:";
        worksheet.Cells["A5"].Style.Font.Bold = true;
        worksheet.Cells["A5"].Style.Font.Size = 11;
        
        worksheet.Cells["A6"].Value = "• Fields marked with * are required / Los campos marcados con * son obligatorios";
        worksheet.Cells["A7"].Value = "• Year: Must be between 1900 and current year + 1 / Año: Debe estar entre 1900 y el año actual + 1";
        worksheet.Cells["A8"].Value = "• Vehicle Type / Tipo de Vehículo: Excavator, Forklift, DumpTruck, Crane, Backhoe, FrontLoader, Bulldozer, Grader, Roller, Paver, Mixer, Pump, Generator, Compressor, Other";
        worksheet.Cells["A9"].Value = "• At least one rate must be > 0 / Al menos una tarifa debe ser mayor que 0";
        worksheet.Cells["A10"].Value = "• License Plate: Must be unique / La placa debe ser única";
        worksheet.Cells["A11"].Value = "• Column names can be in English or Spanish / Los nombres de columnas pueden estar en inglés o español";
        worksheet.Cells["A12"].Value = "• Accepted names: Brand/Marca, Model/Modelo, Year/Año, License Plate/Placa, Vehicle Type/Tipo, Daily Rate/TarifaDiaria, etc.";
        worksheet.Cells["A13"].Value = "• Delete the example rows before importing / Elimine las filas de ejemplo antes de importar";
        
        using (var range = worksheet.Cells["A6:A13"])
        {
            range.Style.Font.Italic = true;
            range.Style.Font.Color.SetColor(Color.FromArgb(128, 128, 128));
        }

        // Freeze header row
        worksheet.View.FreezePanes(2, 1);

        return package.GetAsByteArray();
    }

    public byte[] GenerateCustomerTemplate()
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Customers");

        // Configure column widths
        worksheet.Column(1).Width = 20;  // First Name
        worksheet.Column(2).Width = 20;  // Last Name
        worksheet.Column(3).Width = 30;  // Email
        worksheet.Column(4).Width = 15;  // Document
        worksheet.Column(5).Width = 15;  // Phone
        worksheet.Column(6).Width = 40;  // Address

        // Header row styling
        using (var range = worksheet.Cells["A1:F1"])
        {
            range.Style.Font.Bold = true;
            range.Style.Font.Size = 12;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(237, 125, 49)); // Orange
            range.Style.Font.Color.SetColor(Color.White);
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Row(1).Height = 25;
        }

        // Headers
        worksheet.Cells["A1"].Value = "First Name *";
        worksheet.Cells["B1"].Value = "Last Name";
        worksheet.Cells["C1"].Value = "Email *";
        worksheet.Cells["D1"].Value = "Document";
        worksheet.Cells["E1"].Value = "Phone";
        worksheet.Cells["F1"].Value = "Address";

        // Add example rows
        worksheet.Cells["A2"].Value = "John";
        worksheet.Cells["B2"].Value = "Doe";
        worksheet.Cells["C2"].Value = "john.doe@example.com";
        worksheet.Cells["D2"].Value = "12345678";
        worksheet.Cells["E2"].Value = "+1-555-0100";
        worksheet.Cells["F2"].Value = "123 Main St, City, State 12345";

        worksheet.Cells["A3"].Value = "Jane";
        worksheet.Cells["B3"].Value = "Smith";
        worksheet.Cells["C3"].Value = "jane.smith@example.com";
        worksheet.Cells["D3"].Value = "87654321";
        worksheet.Cells["E3"].Value = "+1-555-0200";
        worksheet.Cells["F3"].Value = "456 Oak Ave, City, State 67890";

        // Style example rows
        using (var range = worksheet.Cells["A2:F3"])
        {
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(252, 228, 214)); // Light orange
        }

        // Add notes
        worksheet.Cells["A5"].Value = "INSTRUCTIONS:";
        worksheet.Cells["A5"].Style.Font.Bold = true;
        worksheet.Cells["A5"].Style.Font.Size = 11;
        
        worksheet.Cells["A6"].Value = "• Fields marked with * are required";
        worksheet.Cells["A7"].Value = "• Email: Must be unique and valid format";
        worksheet.Cells["A8"].Value = "• If customer exists (same email), data will be updated";
        worksheet.Cells["A9"].Value = "• Delete the example rows before importing";
        
        using (var range = worksheet.Cells["A6:A9"])
        {
            range.Style.Font.Italic = true;
            range.Style.Font.Color.SetColor(Color.FromArgb(128, 128, 128));
        }

        // Freeze header row
        worksheet.View.FreezePanes(2, 1);

        return package.GetAsByteArray();
    }

    public byte[] GenerateSampleDataFile()
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Sample Data - Datos de Ejemplo");

        // Configure column widths
        worksheet.Column(1).Width = 15;
        worksheet.Column(2).Width = 35;
        worksheet.Column(3).Width = 45;
        worksheet.Column(4).Width = 12;
        worksheet.Column(5).Width = 10;
        worksheet.Column(6).Width = 50;
        worksheet.Column(7).Width = 25;

        // Header row styling
        using (var range = worksheet.Cells["A1:G1"])
        {
            range.Style.Font.Bold = true;
            range.Style.Font.Size = 12;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 192, 0)); // Orange/Gold
            range.Style.Font.Color.SetColor(Color.White);
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Row(1).Height = 30;
        }

        // Headers (deliberately mixed languages to show flexibility)
        worksheet.Cells["A1"].Value = "Codigo";
        worksheet.Cells["B1"].Value = "Nombre";
        worksheet.Cells["C1"].Value = "Description";
        worksheet.Cells["D1"].Value = "Precio";
        worksheet.Cells["E1"].Value = "Stock";
        worksheet.Cells["F1"].Value = "Imagen";
        worksheet.Cells["G1"].Value = "Category";

        // Sample data - INTENTIONALLY MESSY AND UNORDERED
        var sampleData = new[]
        {
            new { SKU = "CEM-001", Name = "Cemento Portland Tipo I", Desc = "Cemento gris de alta resistencia", Price = 25.50m, Stock = 150m, Image = "", Category = "Materiales de Construcción" },
            new { SKU = "", Name = "Varilla de Acero 3/8\"", Desc = "Varilla corrugada para refuerzo", Price = 12.75m, Stock = 500m, Image = "https://example.com/varilla.jpg", Category = "Acero y Metales" },
            new { SKU = "LAD-005", Name = "Ladrillo Rojo Común", Desc = "", Price = 0.35m, Stock = 10000m, Image = "", Category = "Materiales de Construcción" },
            new { SKU = "PIN-TEC-001", Name = "Pintura Latex Interior Blanco", Desc = "Pintura lavable de alta cobertura", Price = 45.00m, Stock = 80m, Image = "https://example.com/pintura.jpg", Category = "Pinturas" },
            new { SKU = "", Name = "Arena Fina", Desc = "Arena cernida para construcción", Price = 15.00m, Stock = 200m, Image = "", Category = "Agregados" },
            new { SKU = "GRA-001", Name = "Grava 3/4\"", Desc = "Piedra triturada para concreto", Price = 18.50m, Stock = 180m, Image = "", Category = "Agregados" },
            new { SKU = "TUB-PVC-4", Name = "Tubería PVC 4 pulgadas", Desc = "Tubo sanitario PVC", Price = 8.90m, Stock = 250m, Image = "", Category = "Plomería" },
            new { SKU = "CAB-12", Name = "Cable Eléctrico #12 AWG", Desc = "Cable de cobre para instalaciones", Price = 2.50m, Stock = 1000m, Image = "https://example.com/cable.jpg", Category = "Electricidad" },
            new { SKU = "", Name = "Cemento Blanco", Desc = "Cemento para acabados finos", Price = 32.00m, Stock = 60m, Image = "", Category = "Materiales de Construcción" },
            new { SKU = "HER-MAR-01", Name = "Martillo de Uña 16oz", Desc = "Martillo con mango de fibra de vidrio", Price = 18.75m, Stock = 45m, Image = "", Category = "Herramientas" },
            new { SKU = "TOR-M10", Name = "Tornillos M10 x 50mm (Caja 100 unidades)", Desc = "Tornillos hexagonales galvanizados", Price = 12.00m, Stock = 120m, Image = "", Category = "Ferretería" },
            new { SKU = "", Name = "Pegamento para PVC", Desc = "Adhesivo para tubería PVC", Price = 6.50m, Stock = 200m, Image = "", Category = "Plomería" },
            new { SKU = "MAD-TAB-2X4", Name = "Tabla de Pino 2x4x8'", Desc = "Madera tratada para construcción", Price = 8.25m, Stock = 300m, Image = "", Category = "Madera" },
            new { SKU = "YE-001", Name = "Yeso en Polvo 25kg", Desc = "Yeso para acabados interiores", Price = 9.80m, Stock = 150m, Image = "", Category = "Materiales de Construcción" },
            new { SKU = "BRO-4IN", Name = "Brocha 4 pulgadas", Desc = "Brocha profesional para pintura", Price = 5.50m, Stock = 85m, Image = "", Category = "Pinturas" }
        };

        int row = 2;
        foreach (var item in sampleData)
        {
            worksheet.Cells[row, 1].Value = item.SKU;
            worksheet.Cells[row, 2].Value = item.Name;
            worksheet.Cells[row, 3].Value = item.Desc;
            worksheet.Cells[row, 4].Value = item.Price;
            worksheet.Cells[row, 5].Value = item.Stock;
            worksheet.Cells[row, 6].Value = item.Image;
            worksheet.Cells[row, 7].Value = item.Category;
            
            // Alternate row colors for better readability
            if (row % 2 == 0)
            {
                using (var range = worksheet.Cells[row, 1, row, 7])
                {
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(252, 248, 227)); // Very light yellow
                }
            }
            
            row++;
        }

        // Add note at the bottom
        worksheet.Cells[$"A{row + 2}"].Value = "NOTA / NOTE:";
        worksheet.Cells[$"A{row + 2}"].Style.Font.Bold = true;
        worksheet.Cells[$"A{row + 3}"].Value = "• Este archivo contiene datos de ejemplo con columnas en español e inglés mezcladas";
        worksheet.Cells[$"A{row + 4}"].Value = "• This file contains sample data with mixed Spanish and English column names";
        worksheet.Cells[$"A{row + 5}"].Value = "• El sistema organizará automáticamente los datos sin importar el orden o idioma";
        worksheet.Cells[$"A{row + 6}"].Value = "• The system will automatically organize the data regardless of order or language";
        worksheet.Cells[$"A{row + 7}"].Value = "• Las categorías que no existan se crearán automáticamente";
        worksheet.Cells[$"A{row + 8}"].Value = "• Categories that don't exist will be created automatically";

        using (var range = worksheet.Cells[$"A{row + 3}:A{row + 8}"])
        {
            range.Style.Font.Italic = true;
            range.Style.Font.Color.SetColor(Color.FromArgb(128, 128, 128));
        }

        // Freeze header row
        worksheet.View.FreezePanes(2, 1);

        return package.GetAsByteArray();
    }

    public byte[] GenerateCategoriesReference()
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Categories - Categorías");

        // Configure column widths
        worksheet.Column(1).Width = 30;
        worksheet.Column(2).Width = 50;
        worksheet.Column(3).Width = 60;

        // Header row styling
        using (var range = worksheet.Cells["A1:C1"])
        {
            range.Style.Font.Bold = true;
            range.Style.Font.Size = 12;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(91, 155, 213)); // Blue
            range.Style.Font.Color.SetColor(Color.White);
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Row(1).Height = 25;
        }

        worksheet.Cells["A1"].Value = "Category / Categoría";
        worksheet.Cells["B1"].Value = "Description / Descripción";
        worksheet.Cells["C1"].Value = "Examples / Ejemplos";

        // Common construction categories
        var categories = new[]
        {
            new { Name = "Materiales de Construcción / Construction Materials", Desc = "Cemento, arena, grava, bloques / Cement, sand, gravel, blocks", Examples = "Cemento Portland, Bloques de Concreto, Arena Fina" },
            new { Name = "Acero y Metales / Steel and Metals", Desc = "Varillas, perfiles, láminas / Rebars, profiles, sheets", Examples = "Varilla 3/8\", Perfil Metálico, Lámina Galvanizada" },
            new { Name = "Pinturas / Paints", Desc = "Pinturas, barnices, selladores / Paints, varnishes, sealers", Examples = "Pintura Latex, Barniz, Sellador Acrílico" },
            new { Name = "Plomería / Plumbing", Desc = "Tuberías, conexiones, válvulas / Pipes, fittings, valves", Examples = "Tubería PVC, Codos, Válvulas" },
            new { Name = "Electricidad / Electrical", Desc = "Cables, interruptores, tomacorrientes / Wires, switches, outlets", Examples = "Cable #12, Interruptores, Tomacorrientes" },
            new { Name = "Herramientas / Tools", Desc = "Herramientas manuales y eléctricas / Hand and power tools", Examples = "Martillos, Taladros, Llaves" },
            new { Name = "Ferretería / Hardware", Desc = "Tornillos, clavos, bisagras / Screws, nails, hinges", Examples = "Tornillos, Clavos, Bisagras" },
            new { Name = "Madera / Wood", Desc = "Tablas, vigas, triplay / Boards, beams, plywood", Examples = "Tabla 2x4, Triplay, Vigas" },
            new { Name = "Agregados / Aggregates", Desc = "Arena, grava, piedra / Sand, gravel, stone", Examples = "Arena Gruesa, Grava 3/4\", Piedra Triturada" },
            new { Name = "Acabados / Finishes", Desc = "Pisos, azulejos, molduras / Flooring, tiles, moldings", Examples = "Cerámica, Porcelanato, Molduras" }
        };

        int row = 2;
        foreach (var cat in categories)
        {
            worksheet.Cells[row, 1].Value = cat.Name;
            worksheet.Cells[row, 2].Value = cat.Desc;
            worksheet.Cells[row, 3].Value = cat.Examples;
            
            // Alternate row colors
            if (row % 2 == 0)
            {
                using (var range = worksheet.Cells[row, 1, row, 3])
                {
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(217, 225, 242)); // Light blue
                }
            }
            
            row++;
        }

        // Add notes
        worksheet.Cells[$"A{row + 2}"].Value = "IMPORTANT / IMPORTANTE:";
        worksheet.Cells[$"A{row + 2}"].Style.Font.Bold = true;
        worksheet.Cells[$"A{row + 2}"].Style.Font.Size = 11;
        
        worksheet.Cells[$"A{row + 3}"].Value = "• You can use any of these categories in your import file";
        worksheet.Cells[$"A{row + 4}"].Value = "• Puede usar cualquiera de estas categorías en su archivo de importación";
        worksheet.Cells[$"A{row + 5}"].Value = "• If you use a category that doesn't exist, it will be created automatically";
        worksheet.Cells[$"A{row + 6}"].Value = "• Si usa una categoría que no existe, se creará automáticamente";
        worksheet.Cells[$"A{row + 7}"].Value = "• Categories are case-insensitive (Pinturas = pinturas = PINTURAS)";
        worksheet.Cells[$"A{row + 8}"].Value = "• Las categorías no distinguen mayúsculas/minúsculas";

        using (var range = worksheet.Cells[$"A{row + 3}:A{row + 8}"])
        {
            range.Style.Font.Italic = true;
            range.Style.Font.Color.SetColor(Color.FromArgb(128, 128, 128));
        }

        // Freeze header row
        worksheet.View.FreezePanes(2, 1);

        return package.GetAsByteArray();
    }
}
