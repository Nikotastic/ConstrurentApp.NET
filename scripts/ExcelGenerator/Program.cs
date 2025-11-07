using OfficeOpenXml;

// Configurar licencia NonCommercial para EPPlus
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

var fileName = "Datos_Prueba_Importacion.xlsx";
var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

using (var package = new ExcelPackage(new FileInfo(filePath)))
{
    var worksheet = package.Workbook.Worksheets.Add("Datos Mezclados");

    // Encabezados
    worksheet.Cells[1, 1].Value = "SKU";
    worksheet.Cells[1, 2].Value = "Producto";
    worksheet.Cells[1, 3].Value = "Descripcion";
    worksheet.Cells[1, 4].Value = "Precio";
    worksheet.Cells[1, 5].Value = "Stock";
    worksheet.Cells[1, 6].Value = "Nombre";
    worksheet.Cells[1, 7].Value = "Apellido";
    worksheet.Cells[1, 8].Value = "Email";
    worksheet.Cells[1, 9].Value = "Telefono";
    worksheet.Cells[1, 10].Value = "Direccion";
    worksheet.Cells[1, 11].Value = "Documento";
    worksheet.Cells[1, 12].Value = "Cantidad";
    worksheet.Cells[1, 13].Value = "Fecha";

    // Estilo de encabezados
    using (var range = worksheet.Cells[1, 1, 1, 13])
    {
        range.Style.Font.Bold = true;
        range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));
        range.Style.Font.Color.SetColor(System.Drawing.Color.White);
        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
    }

    int row = 2;

    // Fila 1: Laptop + Cliente Juan + Venta
    worksheet.Cells[row, 1].Value = "PROD001";
    worksheet.Cells[row, 2].Value = "Laptop HP Pavilion 15";
    worksheet.Cells[row, 3].Value = "Laptop 15.6 pulgadas, Intel i7, 16GB RAM, 512GB SSD";
    worksheet.Cells[row, 4].Value = 850.00;
    worksheet.Cells[row, 5].Value = 10;
    worksheet.Cells[row, 6].Value = "Juan";
    worksheet.Cells[row, 7].Value = "Pérez García";
    worksheet.Cells[row, 8].Value = "juan.perez@email.com";
    worksheet.Cells[row, 9].Value = "555-1234";
    worksheet.Cells[row, 10].Value = "Calle Principal 123, Ciudad";
    worksheet.Cells[row, 11].Value = "12345678A";
    worksheet.Cells[row, 12].Value = 2;
    worksheet.Cells[row, 13].Value = "2025-11-07";
    row++;

    // Fila 2: Mouse + Cliente María + Venta
    worksheet.Cells[row, 1].Value = "PROD002";
    worksheet.Cells[row, 2].Value = "Mouse Logitech MX Master 3";
    worksheet.Cells[row, 3].Value = "Mouse inalámbrico ergonómico de alta precisión";
    worksheet.Cells[row, 4].Value = 99.99;
    worksheet.Cells[row, 5].Value = 25;
    worksheet.Cells[row, 6].Value = "María";
    worksheet.Cells[row, 7].Value = "González López";
    worksheet.Cells[row, 8].Value = "maria.gonzalez@email.com";
    worksheet.Cells[row, 9].Value = "555-5678";
    worksheet.Cells[row, 10].Value = "Av. Libertad 456, Ciudad";
    worksheet.Cells[row, 11].Value = "87654321B";
    worksheet.Cells[row, 12].Value = 1;
    worksheet.Cells[row, 13].Value = "2025-11-07";
    row++;

    // Fila 3: Teclado + Cliente Carlos + Venta
    worksheet.Cells[row, 1].Value = "PROD003";
    worksheet.Cells[row, 2].Value = "Teclado Mecánico Corsair K95";
    worksheet.Cells[row, 3].Value = "Teclado mecánico RGB, switches Cherry MX";
    worksheet.Cells[row, 4].Value = 179.99;
    worksheet.Cells[row, 5].Value = 15;
    worksheet.Cells[row, 6].Value = "Carlos";
    worksheet.Cells[row, 7].Value = "Rodríguez Martínez";
    worksheet.Cells[row, 8].Value = "carlos.rodriguez@email.com";
    worksheet.Cells[row, 9].Value = "555-9012";
    worksheet.Cells[row, 10].Value = "Calle Sol 789, Ciudad";
    worksheet.Cells[row, 11].Value = "11223344C";
    worksheet.Cells[row, 12].Value = 1;
    worksheet.Cells[row, 13].Value = "2025-11-06";
    row++;

    // Fila 4: Monitor + Cliente Ana + Venta
    worksheet.Cells[row, 1].Value = "PROD004";
    worksheet.Cells[row, 2].Value = "Monitor Dell 27 4K";
    worksheet.Cells[row, 3].Value = "Monitor 27 pulgadas, resolución 4K, IPS";
    worksheet.Cells[row, 4].Value = 450.00;
    worksheet.Cells[row, 5].Value = 8;
    worksheet.Cells[row, 6].Value = "Ana";
    worksheet.Cells[row, 7].Value = "Fernández Ruiz";
    worksheet.Cells[row, 8].Value = "ana.fernandez@email.com";
    worksheet.Cells[row, 9].Value = "555-3456";
    worksheet.Cells[row, 10].Value = "Plaza Mayor 321, Ciudad";
    worksheet.Cells[row, 11].Value = "55667788D";
    worksheet.Cells[row, 12].Value = 1;
    worksheet.Cells[row, 13].Value = "2025-11-05";
    row++;

    // Fila 5: Juan compra otro producto (Mouse) - Cliente repetido
    worksheet.Cells[row, 1].Value = "PROD002";
    worksheet.Cells[row, 2].Value = "Mouse Logitech MX Master 3";
    worksheet.Cells[row, 3].Value = "Mouse inalámbrico ergonómico de alta precisión";
    worksheet.Cells[row, 4].Value = 99.99;
    worksheet.Cells[row, 5].Value = 25;
    worksheet.Cells[row, 6].Value = "Juan";
    worksheet.Cells[row, 7].Value = "Pérez García";
    worksheet.Cells[row, 8].Value = "juan.perez@email.com";
    worksheet.Cells[row, 9].Value = "555-1234";
    worksheet.Cells[row, 10].Value = "Calle Principal 123, Ciudad";
    worksheet.Cells[row, 11].Value = "12345678A";
    worksheet.Cells[row, 12].Value = 3;
    worksheet.Cells[row, 13].Value = "2025-11-07";
    row++;

    // Fila 6: Nuevo producto sin venta (solo inventario)
    worksheet.Cells[row, 1].Value = "PROD005";
    worksheet.Cells[row, 2].Value = "Webcam Logitech C920";
    worksheet.Cells[row, 3].Value = "Webcam Full HD 1080p con micrófono estéreo";
    worksheet.Cells[row, 4].Value = 79.99;
    worksheet.Cells[row, 5].Value = 20;
    row++;

    // Fila 7: Nuevo cliente sin venta
    worksheet.Cells[row, 6].Value = "Pedro";
    worksheet.Cells[row, 7].Value = "Sánchez Torres";
    worksheet.Cells[row, 8].Value = "pedro.sanchez@email.com";
    worksheet.Cells[row, 9].Value = "555-7890";
    worksheet.Cells[row, 10].Value = "Av. Central 654, Ciudad";
    worksheet.Cells[row, 11].Value = "99887766E";
    row++;

    // Fila 8: Auriculares + Cliente Luis + Venta
    worksheet.Cells[row, 1].Value = "PROD006";
    worksheet.Cells[row, 2].Value = "Auriculares Sony WH-1000XM5";
    worksheet.Cells[row, 3].Value = "Auriculares con cancelación de ruido activa";
    worksheet.Cells[row, 4].Value = 349.99;
    worksheet.Cells[row, 5].Value = 12;
    worksheet.Cells[row, 6].Value = "Luis";
    worksheet.Cells[row, 7].Value = "Martínez Díaz";
    worksheet.Cells[row, 8].Value = "luis.martinez@email.com";
    worksheet.Cells[row, 9].Value = "555-1111";
    worksheet.Cells[row, 10].Value = "Calle Luna 222, Ciudad";
    worksheet.Cells[row, 11].Value = "44556677F";
    worksheet.Cells[row, 12].Value = 1;
    worksheet.Cells[row, 13].Value = "2025-11-04";
    row++;

    // Fila 9: María compra Monitor (venta de producto existente a cliente existente)
    worksheet.Cells[row, 1].Value = "PROD004";
    worksheet.Cells[row, 6].Value = "María";
    worksheet.Cells[row, 7].Value = "González López";
    worksheet.Cells[row, 8].Value = "maria.gonzalez@email.com";
    worksheet.Cells[row, 12].Value = 2;
    worksheet.Cells[row, 13].Value = "2025-11-07";
    row++;

    // Fila 10: Impresora + Cliente Elena + Venta
    worksheet.Cells[row, 1].Value = "PROD007";
    worksheet.Cells[row, 2].Value = "Impresora HP LaserJet Pro";
    worksheet.Cells[row, 3].Value = "Impresora láser monocromática, WiFi";
    worksheet.Cells[row, 4].Value = 229.99;
    worksheet.Cells[row, 5].Value = 5;
    worksheet.Cells[row, 6].Value = "Elena";
    worksheet.Cells[row, 7].Value = "Ramírez Castro";
    worksheet.Cells[row, 8].Value = "elena.ramirez@email.com";
    worksheet.Cells[row, 9].Value = "555-2222";
    worksheet.Cells[row, 10].Value = "Av. Norte 888, Ciudad";
    worksheet.Cells[row, 11].Value = "33221100G";
    worksheet.Cells[row, 12].Value = 1;
    worksheet.Cells[row, 13].Value = "2025-11-03";

    // Ajustar anchos de columna
    worksheet.Column(1).Width = 12;
    worksheet.Column(2).Width = 30;
    worksheet.Column(3).Width = 45;
    worksheet.Column(4).Width = 10;
    worksheet.Column(5).Width = 8;
    worksheet.Column(6).Width = 15;
    worksheet.Column(7).Width = 20;
    worksheet.Column(8).Width = 30;
    worksheet.Column(9).Width = 12;
    worksheet.Column(10).Width = 35;
    worksheet.Column(11).Width = 12;
    worksheet.Column(12).Width = 10;
    worksheet.Column(13).Width = 12;

    // Guardar archivo
    package.SaveAs(new FileInfo(filePath));
}

Console.WriteLine($"✅ Archivo creado exitosamente: {filePath}");
Console.WriteLine($"📊 El archivo contiene 10 filas con datos desnormalizados mezclados");
Console.WriteLine($"📦 Productos: 7 productos diferentes");
Console.WriteLine($"👥 Clientes: 7 clientes diferentes");
Console.WriteLine($"🛒 Ventas: 8 ventas registradas");
Console.WriteLine();
Console.WriteLine("Contenido del archivo:");
Console.WriteLine("- Datos mezclados de productos, clientes y ventas en las mismas filas");
Console.WriteLine("- Clientes repetidos para probar actualizaciones");
Console.WriteLine("- Productos sin ventas para probar carga de inventario");
Console.WriteLine("- Clientes sin ventas para probar registro individual");
