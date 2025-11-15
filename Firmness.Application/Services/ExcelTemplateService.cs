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
        worksheet.Cells[1, 10].Value = "Name";
        worksheet.Cells[1, 11].Value = "Last name";
        worksheet.Cells[1, 12].Value = "Email";
        worksheet.Cells[1, 13].Value = "Phone";
        worksheet.Cells[1, 14].Value = "Address";
        worksheet.Cells[1, 15].Value = "Document";
        worksheet.Cells[1, 16].Value = "Amount";

        // Header styling
        using (var range = worksheet.Cells[1, 1, 1, 16])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));
            range.Style.Font.Color.SetColor(System.Drawing.Color.White);
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            range.Style.Border.BorderAround(ExcelBorderStyle.Medium);
        }

        // Add instructions
        worksheet.Cells["A3"].Value = "INSTRUCTIONS:";
        worksheet.Cells["A3"].Style.Font.Bold = true;
        worksheet.Cells["A3"].Style.Font.Size = 12;
        worksheet.Cells["A3"].Style.Font.Color.SetColor(System.Drawing.Color.DarkBlue);
        
        worksheet.Cells["A4"].Value = "• You can leave columns empty if you don't apply to that row";
        worksheet.Cells["A5"].Value = "• The system automatically identifies whether they are products, customers, or sales";
        worksheet.Cells["A6"].Value = "• Products are searched by SKU, customers by Email or Document";
        worksheet.Cells["A7"].Value = "• If it exists, it is updated; Otherwise, it is created";
        worksheet.Cells["A8"].Value = "• Required fields:";
        worksheet.Cells["A9"].Value = "- Products: SKU, Product, Price";
        worksheet.Cells["A10"].Value = "- Customers: First Name, Last Name, Email, ID";
        worksheet.Cells["A11"].Value = "- Sales: Product SKU, Customer Email, Quantity";
        worksheet.Cells["A12"].Value = "• The category must exactly match one of the available categories (see 'Categories' sheet)";
        using (var instrRange = worksheet.Cells["A4:A12"])
        {
            instrRange.Style.Font.Size = 10;
            instrRange.Style.WrapText = true;
        }

        // Auto-fit columns
        worksheet.Cells.AutoFitColumns();
        
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
            "SKU", "Product","Description", "Price", "Stock", "Cost", "Minimum Stock", "Barcode", "Category", "First Name", "Last Name","Email", "Phone", "Address"," ID Number", "Quantity"
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

        // Ejemplo 1: Cemento (usando categoría exacta del sistema)
        AddRow(worksheet, row++, 
            "CONST-001", "Cemento Portland Gris x 50kg", "Cemento tipo I para construcción general", 
            25.50m, 18.00m, 200, 50, "7701234567890", "Cement and Concrete (Cemento y Concreto)");

        // Ejemplo 2: Producto + Cliente + Venta (herramientas eléctricas)
        AddRow(worksheet, row++, 
            "CONST-002", "Taladro Percutor DeWalt 850W", "Taladro eléctrico profesional con estuche y brocas", 
            185.99m, 120.00m, 35, 10, "7701234567891", "Power Tools (Herramientas Eléctricas)",
            "Roberto", "Martínez", "roberto.martinez@construcciones.com", "+57 310 456 7890", 
            "Carrera 15 #85-32, Bogotá", "52123456", 2);

        // Ejemplo 3: Pisos y azulejos
        AddRow(worksheet, row++, 
            "CONST-003", "Cerámica Piso 45x45cm Beige", "Cerámica antideslizante para piso, caja x 2.03 m²", 
            42.99m, 28.50m, 150, 30, "7701234567892", "Floors and Tiles (Pisos y Azulejos)");

        // Ejemplo 4: Cliente nuevo (constructor)
        AddRow(worksheet, row++, 
            null, null, null, null, null, null, null, null, null,
            "Ana", "Gómez", "ana.gomez@email.com", "+57 315 678 9012", 
            "Calle 127 #45-67, Bogotá", "1098765432");

        // Ejemplo 5: Venta de cemento a cliente nuevo
        AddRow(worksheet, row++, 
            "CONST-001", null, null, null, null, null, null, null, null,
            "Ana", "Gómez", "ana.gomez@email.com", null, null, null, 50);

        // Ejemplo 6: Varilla y acero
        AddRow(worksheet, row++, 
            "CONST-004", "Varilla Corrugada 3/8\" x 6m", "Varilla de acero corrugado grado 60", 
            12.75m, 9.20m, 500, 100, "7701234567893", "Rebar and Steel (Varilla y Acero)");

        // Ejemplo 7: Cliente + venta de varillas (empresa constructora)
        AddRow(worksheet, row++, 
            "CONST-004", null, null, null, null, null, null, null, null,
            "Construcciones", "Pérez S.A.S", "compras@construccionesperez.com", "+57 601 234 5678", 
            "Autopista Norte #145-30, Bogotá", "900123456", 100);

        // Ejemplo 8: Herramienta manual
        AddRow(worksheet, row++, 
            "CONST-005", "Nivel Láser Autonivelante", "Nivel láser de línea cruzada, alcance 30m", 
            145.00m, 95.00m, 25, 8, "7701234567894", "Hand Tools (Herramientas Manuales)");

        // Ejemplo 9: Actualizar cliente existente (mismo email)
        AddRow(worksheet, row++, 
            null, null, null, null, null, null, null, null, null,
            "Roberto", "Martínez Vargas", "roberto.martinez@construcciones.com", "+57 310 999 8888", 
            "Carrera 15 #85-32 Oficina 201, Bogotá", "52123456");

        // Ejemplo 10: Material eléctrico
        AddRow(worksheet, row++, 
            "CONST-006", "Cable Eléctrico THW 12 AWG x 100m", "Cable de cobre calibre 12 para instalaciones eléctricas", 
            78.50m, 52.00m, 80, 20, "7701234567895", "Electrical (Electricidad)");

        // Ejemplo 11: Tubería PVC
        AddRow(worksheet, row++, 
            "CONST-007", "Tubería PVC 4\" x 3m Sanitaria", "Tubería PVC para desagües y alcantarillado", 
            18.90m, 12.50m, 120, 25, "7701234567896", "Pipe and PVC (Tubería y PVC)");

        // Ejemplo 12: Venta de cerámica a cliente existente
        AddRow(worksheet, row++, 
            "CONST-003", null, null, null, null, null, null, null, null,
            "Ana", "Gómez", "ana.gomez@email.com", null, null, null, 20);

        // Ejemplo 13: Pintura
        AddRow(worksheet, row++, 
            "CONST-008", "Pintura Vinílica Blanco 20L", "Pintura lavable para interiores y exteriores", 
            45.99m, 32.00m, 60, 15, "7701234567897", "Paints and Coatings (Pinturas y Recubrimientos)");

        // Ejemplo 14: Renta de maquinaria - Montacargas
        AddRow(worksheet, row++, 
            "RENT-001", "Montacargas Eléctrico 2.5 Ton", "Montacargas eléctrico para renta diaria/mensual", 
            150.00m, 100.00m, 5, 2, "7701234567898", "Forklifts (Montacargas)");

        // Auto-fit columns
        worksheet.Cells.AutoFitColumns();
        
        // Adjust specific column widths
        worksheet.Column(2).Width = 35;  // Producto
        worksheet.Column(3).Width = 50;  // Descripcion
        worksheet.Column(9).Width = 40;  // Categoria (más ancho para nombres completos)
        worksheet.Column(12).Width = 35; // Email
        worksheet.Column(14).Width = 40; // Direccion
        
        // Add note at bottom
        worksheet.Cells[row + 2, 1].Value = "NOTA: Este archivo contiene datos de ejemplo con categorías reales del sistema. Ver hoja 'Categorías Disponibles' para lista completa.";
        worksheet.Cells[row + 2, 1].Style.Font.Italic = true;
        worksheet.Cells[row + 2, 1].Style.Font.Color.SetColor(System.Drawing.Color.Gray);
        using (var range = worksheet.Cells[row + 2, 1, row + 2, headers.Length])
        {
            range.Merge = true;
        }
        
        // Add Categories sheet
        AddCategoriesSheet(package);
        
        return package.GetAsByteArray();
    }

    private void AddCategoriesSheet(ExcelPackage package)
    {
        var catSheet = package.Workbook.Worksheets.Add("Categorías Disponibles");
        
        // Title
        catSheet.Cells["A1"].Value = "CATEGORÍAS DISPONIBLES EN EL SISTEMA";
        catSheet.Cells["A1"].Style.Font.Bold = true;
        catSheet.Cells["A1"].Style.Font.Size = 14;
        catSheet.Cells["A1"].Style.Font.Color.SetColor(System.Drawing.Color.DarkBlue);
        using (var range = catSheet.Cells["A1:B1"])
        {
            range.Merge = true;
        }
        
        // Headers
        catSheet.Cells["A3"].Value = "Nombre de Categoría (usar exactamente como aparece)";
        catSheet.Cells["B3"].Value = "Descripción";
        
        using (var range = catSheet.Cells["A3:B3"])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));
            range.Style.Font.Color.SetColor(System.Drawing.Color.White);
        }
        
        int row = 4;
        
        // Construction Materials Section
        catSheet.Cells[row, 1].Value = "MATERIALES DE CONSTRUCCIÓN";
        catSheet.Cells[row, 1].Style.Font.Bold = true;
        catSheet.Cells[row, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
        catSheet.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
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
        catSheet.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
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
        catSheet.Cells.AutoFitColumns();
        catSheet.Column(1).Width = 50;
        catSheet.Column(2).Width = 70;
        
        // Add note
        row += 2;
        catSheet.Cells[row, 1].Value = "IMPORTANTE: Al importar productos, el nombre de la categoría debe coincidir EXACTAMENTE con uno de los nombres de esta lista.";
        catSheet.Cells[row, 1].Style.Font.Bold = true;
        catSheet.Cells[row, 1].Style.Font.Color.SetColor(System.Drawing.Color.Red);
        using (var range = catSheet.Cells[row, 1, row, 2])
        {
            range.Merge = true;
            range.Style.WrapText = true;
        }
    }

    private void AddRow(ExcelWorksheet ws, int row,
        string? sku = null, string? producto = null, string? descripcion = null,
        decimal? precio = null, decimal? costo = null, int? stock = null, int? stockMin = null,
        string? barcode = null, string? categoria = null,
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
        ws.Cells[row, col++].Value = nombre;
        ws.Cells[row, col++].Value = apellido;
        ws.Cells[row, col++].Value = email;
        ws.Cells[row, col++].Value = telefono;
        ws.Cells[row, col++].Value = direccion;
        ws.Cells[row, col++].Value = documento;
        ws.Cells[row, col++].Value = cantidad;
    }
}
