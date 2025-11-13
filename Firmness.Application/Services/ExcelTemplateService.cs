using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Firmness.Application.Services;

public class ExcelTemplateService : IExcelTemplateService
{
    public byte[] GenerateTemplate()
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Data");

        // Headers
        worksheet.Cells[1, 1].Value = "SKU";
        worksheet.Cells[1, 2].Value = "Product";
        worksheet.Cells[1, 3].Value = "Description";
        worksheet.Cells[1, 4].Value = "Price";
        worksheet.Cells[1, 5].Value = "Stock";
        worksheet.Cells[1, 6].Value = "FirstName";
        worksheet.Cells[1, 7].Value = "LastName";
        worksheet.Cells[1, 8].Value = "Email";
        worksheet.Cells[1, 9].Value = "Phone";
        worksheet.Cells[1, 10].Value = "Address";
        worksheet.Cells[1, 11].Value = "Document";
        worksheet.Cells[1, 12].Value = "Quantity";

        // Header styling
        using (var range = worksheet.Cells[1, 1, 1, 12])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));
            range.Style.Font.Color.SetColor(System.Drawing.Color.White);
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            range.Style.Border.BorderAround(ExcelBorderStyle.Medium);
        }

        // Ejemplo 1: Insumo de construcción + Cliente + Venta
        worksheet.Cells[2, 1].Value = "CEM-001";
        worksheet.Cells[2, 2].Value = "Cemento Portland Tipo I - 50kg";
        worksheet.Cells[2, 3].Value = "Cemento gris para construcción general, bolsa de 50kg";
        worksheet.Cells[2, 4].Value = 28.50m;
        worksheet.Cells[2, 5].Value = 500;
        worksheet.Cells[2, 6].Value = "Juan";
        worksheet.Cells[2, 7].Value = "Pérez";
        worksheet.Cells[2, 8].Value = "juan.perez@construcciones.com";
        worksheet.Cells[2, 9].Value = "555-1234";
        worksheet.Cells[2, 10].Value = "Av. Los Constructores 123";
        worksheet.Cells[2, 11].Value = "12345678A";
        worksheet.Cells[2, 12].Value = 50;

        // Ejemplo 2: Solo producto (vehículo industrial)
        worksheet.Cells[3, 1].Value = "VEH-001";
        worksheet.Cells[3, 2].Value = "Retroexcavadora CAT 420F";
        worksheet.Cells[3, 3].Value = "Retroexcavadora Caterpillar, capacidad 1.0 m³, renta por día";
        worksheet.Cells[3, 4].Value = 350.00m;
        worksheet.Cells[3, 5].Value = 3;

        // Ejemplo 3: Venta con producto existente
        worksheet.Cells[4, 1].Value = "CEM-001";
        worksheet.Cells[4, 6].Value = "María";
        worksheet.Cells[4, 7].Value = "González";
        worksheet.Cells[4, 8].Value = "maria.gonzalez@obras.com";
        worksheet.Cells[4, 9].Value = "555-5678";
        worksheet.Cells[4, 12].Value = 100;

        // Ejemplo 4: Solo cliente
        worksheet.Cells[5, 6].Value = "Carlos";
        worksheet.Cells[5, 7].Value = "Rodríguez";
        worksheet.Cells[5, 8].Value = "carlos.rodriguez@edificar.com";
        worksheet.Cells[5, 9].Value = "555-9012";
        worksheet.Cells[5, 10].Value = "Calle Industrial 456";
        worksheet.Cells[5, 11].Value = "87654321B";

        // Instructions
        worksheet.Cells["A15"].Value = "INSTRUCTIONS:";
        worksheet.Cells["A15"].Style.Font.Bold = true;
        worksheet.Cells["A15"].Style.Font.Size = 12;
        
        worksheet.Cells["A16"].Value = "• You can leave columns empty if they don't apply to that row";
        worksheet.Cells["A17"].Value = "• The system automatically identifies what data is products, customers or sales";
        worksheet.Cells["A18"].Value = "• Products are searched by SKU, customers by Email";
        worksheet.Cells["A19"].Value = "• If it exists, it updates; if not, it creates";
        worksheet.Cells["A20"].Value = "• Required fields: Product (name and price), Customer (name and email), Sale (quantity)";
        worksheet.Cells["A21"].Value = "• For sales, SKU, customer Email and Quantity are required";

        // Auto-fit columns
        worksheet.Cells.AutoFitColumns();
        
        // Adjust column widths for better readability
        worksheet.Column(2).Width = 25; // Product
        worksheet.Column(3).Width = 35; // Description
        worksheet.Column(8).Width = 30; // Email
        worksheet.Column(10).Width = 30; // Address
        
        return package.GetAsByteArray();
    }

    public byte[] GenerateSampleData()
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Mixed Data");

        // Headers
        var headers = new[]
        {
            "SKU", "Product", "Description", "Price", "Stock",
            "FirstName", "LastName", "Email", "Phone", "Address",
            "Document", "Quantity"
        };

        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cells[1, i + 1].Value = headers[i];
        }

        // Header styling
        using (var range = worksheet.Cells[1, 1, 1, headers.Length])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));
            range.Style.Font.Color.SetColor(System.Drawing.Color.White);
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            range.Style.Border.BorderAround(ExcelBorderStyle.Medium);
        }

        int row = 2;

        // === INSUMOS DE CONSTRUCCIÓN ===
        
        // Fila 1: Cemento + Cliente Constructor + Venta
        AddRow(worksheet, row++, "CEM-001", "Cemento Portland Tipo I - 50kg", 
            "Cemento gris para construcción general, alta resistencia", 28.50m, 500,
            "Juan", "Pérez López", "juan.perez@construcciones.com", "555-1001", 
            "Av. Los Constructores 123, Zona Industrial", "12345678A", 50);

        // Fila 2: Varillas + Cliente Ingeniero + Venta
        AddRow(worksheet, row++, "VAR-002", "Varilla Corrugada 3/8\" x 6m", 
            "Varilla de acero corrugada grado 60, para estructura", 18.75m, 1000,
            "María", "González Ruiz", "maria.gonzalez@ingarq.com", "555-1002", 
            "Calle Ingeniería 456, Centro", "87654321B", 200);

        // Fila 3: Ladrillos + Cliente Constructor + Venta
        AddRow(worksheet, row++, "LAD-003", "Ladrillo King Kong 18 huecos", 
            "Ladrillo de arcilla cocida, para muros portantes", 0.85m, 10000,
            "Carlos", "Rodríguez Vega", "carlos.rodriguez@edificar.com", "555-1003", 
            "Jr. Construcción 789, Distrito", "11223344C", 5000);

        // Fila 4: Arena + Cliente Obra + Venta
        AddRow(worksheet, row++, "ARE-004", "Arena Gruesa - m³", 
            "Arena gruesa para concreto y mortero, lavada", 45.00m, 200,
            "Ana", "Fernández Castro", "ana.fernandez@obraspe.com", "555-1004", 
            "Av. Las Obras 321, Zona Sur", "55667788D", 15);

        // Fila 5: Piedra Chancada + mismo cliente (Ana) + Venta
        AddRow(worksheet, row++, "PIE-005", "Piedra Chancada 1/2\" - m³", 
            "Piedra chancada de 1/2 pulgada para concreto", 55.00m, 150,
            "Ana", "Fernández Castro", "ana.fernandez@obraspe.com", "555-1004", 
            "Av. Las Obras 321, Zona Sur", "55667788D", 10);

        // === VEHÍCULOS INDUSTRIALES ===
        
        // Fila 6: Retroexcavadora + Cliente Empresa + Renta
        AddRow(worksheet, row++, "VEH-001", "Retroexcavadora CAT 420F", 
            "Retroexcavadora Caterpillar 420F, capacidad 1.0 m³, renta diaria", 350.00m, 3,
            "Luis", "Martínez Soto", "luis.martinez@construccionesmc.com", "555-2001", 
            "Av. Maquinaria 100, Parque Industrial", "44556677E", 7);

        // Fila 7: Volquete + Cliente Empresa + Renta
        AddRow(worksheet, row++, "VEH-002", "Volquete Volvo FMX 8x4 - 15m³", 
            "Volquete Volvo de 15m³, ideal para movimiento de tierra, renta diaria", 280.00m, 5,
            "Pedro", "Sánchez Díaz", "pedro.sanchez@transportesperu.com", "555-2002", 
            "Calle Transporte 200, Zona Norte", "99887766F", 5);

        // Fila 8: Mezcladora + Cliente Constructor + Renta
        AddRow(worksheet, row++, "VEH-003", "Mezcladora de Concreto 9p³", 
            "Mezcladora de concreto portátil 9 pies cúbicos, renta diaria", 85.00m, 8,
            "Juan", "Pérez López", "juan.perez@construcciones.com", null, null, null, 3);

        // === MÁS INSUMOS ===
        
        // Fila 9: Nuevo producto sin venta (solo inventario)
        AddRow(worksheet, row++, "FIE-006", "Fierro Construcción 1/2\" x 9m", 
            "Fierro corrugado de 1/2 pulgada para columnas", 32.50m, 800);

        // Fila 10: Nuevo cliente sin compra
        AddRow(worksheet, row++, null, null, null, null, null,
            "Elena", "Ramírez Torres", "elena.ramirez@constructoraperu.com", "555-3001", 
            "Av. Progreso 888, Distrito Norte", "33221100G");

        // Fila 11: María compra Fierro (cliente existente, producto existente)
        AddRow(worksheet, row++, "FIE-006", null, null, null, null,
            "María", "González Ruiz", "maria.gonzalez@ingarq.com", null, null, null, 150);

        // === MÁS VEHÍCULOS ===
        
        // Fila 12: Compactadora + Nueva Empresa + Renta
        AddRow(worksheet, row++, "VEH-004", "Compactadora Vibrante 10 Ton", 
            "Rodillo compactador vibrante, ideal para asfalto y suelos, renta diaria", 420.00m, 2,
            "Roberto", "Torres Mejía", "roberto.torres@pavimentosperu.com", "555-2003", 
            "Jr. Asfalto 500, Zona Este", "77889900H", 4);

        // Fila 13: Grúa Torre + Cliente Constructora + Renta
        AddRow(worksheet, row++, "VEH-005", "Grúa Torre 50m - 6 Ton", 
            "Grúa torre autopropulsada, alcance 50m, capacidad 6 toneladas, renta mensual", 8500.00m, 1,
            "Constructora", "Edificios SAC", "ventas@constructoraedificios.com", "555-2004", 
            "Av. Construcción 999, Zona Comercial", "66554433I", 1);

        // === MATERIALES ADICIONALES ===
        
        // Fila 14: Tubería PVC + Carlos (cliente existente) + Venta
        AddRow(worksheet, row++, "TUB-007", "Tubería PVC 4\" x 3m Desagüe", 
            "Tubería PVC SAP para desagüe, 4 pulgadas, 3 metros", 22.00m, 600,
            "Carlos", "Rodríguez Vega", "carlos.rodriguez@edificar.com", null, null, null, 80);

        // Fila 15: Alambre + Nueva cliente + Venta
        AddRow(worksheet, row++, "ALA-008", "Alambre Recocido N°16 - kg", 
            "Alambre negro recocido para amarre de fierros", 4.50m, 2000,
            "Patricia", "López Quispe", "patricia.lopez@construccionplq.com", "555-3002", 
            "Calle Materiales 777, Centro", "22334455J", 50);

        // Adjust column widths
        worksheet.Column(1).Width = 12;  // SKU
        worksheet.Column(2).Width = 40;  // Producto
        worksheet.Column(3).Width = 60;  // Descripción
        worksheet.Column(4).Width = 10;  // Precio
        worksheet.Column(5).Width = 8;   // Stock
        worksheet.Column(6).Width = 15;  // Nombre
        worksheet.Column(7).Width = 25;  // Apellido
        worksheet.Column(8).Width = 38;  // Email
        worksheet.Column(9).Width = 12;  // Teléfono
        worksheet.Column(10).Width = 40; // Dirección
        worksheet.Column(11).Width = 12; // Documento
        worksheet.Column(12).Width = 10; // Cantidad

        return package.GetAsByteArray();
    }

    private void AddRow(ExcelWorksheet worksheet, int row,
        string? sku = null, string? product = null, string? description = null, 
        decimal? price = null, int? stock = null,
        string? firstName = null, string? lastName = null, string? email = null, 
        string? phone = null, string? address = null, string? document = null, 
        int? quantity = null)
    {
        if (sku != null) worksheet.Cells[row, 1].Value = sku;
        if (product != null) worksheet.Cells[row, 2].Value = product;
        if (description != null) worksheet.Cells[row, 3].Value = description;
        if (price.HasValue) worksheet.Cells[row, 4].Value = price.Value;
        if (stock.HasValue) worksheet.Cells[row, 5].Value = stock.Value;
        if (firstName != null) worksheet.Cells[row, 6].Value = firstName;
        if (lastName != null) worksheet.Cells[row, 7].Value = lastName;
        if (email != null) worksheet.Cells[row, 8].Value = email;
        if (phone != null) worksheet.Cells[row, 9].Value = phone;
        if (address != null) worksheet.Cells[row, 10].Value = address;
        if (document != null) worksheet.Cells[row, 11].Value = document;
        if (quantity.HasValue) worksheet.Cells[row, 12].Value = quantity.Value;
    }
}
