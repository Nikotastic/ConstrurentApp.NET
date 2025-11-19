using Firmness.Application.Interfaces;
using Firmness.Application.Models;
using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;
using OfficeOpenXml;

namespace Firmness.Application.Services;

public class BulkImportService : IBulkImportService
{
    private readonly IProductRepository _productRepo;
    private readonly ICustomerRepository _customerRepo;
    private readonly ISaleRepository _saleRepo;
    private readonly ISaleItemRepository _saleItemRepo;
    private readonly IVehicleRepository _vehicleRepo;
    private readonly INotificationService _notificationService;

    public BulkImportService(
        IProductRepository productRepo,
        ICustomerRepository customerRepo,
        ISaleRepository saleRepo,
        ISaleItemRepository saleItemRepo,
        IVehicleRepository vehicleRepo,
        INotificationService notificationService)
    {
        _productRepo = productRepo;
        _customerRepo = customerRepo;
        _saleRepo = saleRepo;
        _saleItemRepo = saleItemRepo;
        _vehicleRepo = vehicleRepo;
        _notificationService = notificationService;
    }

    public async Task<BulkImportResult> ImportFromExcelAsync(Stream fileStream, CancellationToken cancellationToken = default)
    {
        var result = new BulkImportResult();
        var newCustomers = new List<Customer>();

        try
        {
            using var package = new ExcelPackage(fileStream);
            var worksheet = package.Workbook.Worksheets[0];
            
            if (worksheet.Dimension == null)
            {
                result.Errors.Add(new ImportError
                {
                    RowNumber = 0,
                    Field = "General",
                    ErrorMessage = "El archivo Excel está vacío"
                });
                return result;
            }

            var headers = ReadHeaders(worksheet);
            var dataType = IdentifyDataType(headers);
            
            result.TotalRows = worksheet.Dimension.Rows - 1;

            // Cargar datos existentes en memoria para búsquedas más rápidas
            var existingProducts = (await _productRepo.GetAllAsync()).ToList();
            var existingCustomers = (await _customerRepo.GetAllAsync()).ToList();

            for (int row = 2; row <= worksheet.Dimension.Rows; row++)
            {
                try
                {
                    var rowData = ReadRowData(worksheet, row, headers);
                    
                    if (IsEmptyRow(rowData))
                        continue;

                    await ProcessRow(rowData, dataType, result, row, existingProducts, existingCustomers, newCustomers);
                    result.SuccessfulRows++;
                }
                catch (Exception ex)
                {
                    result.FailedRows++;
                    result.Errors.Add(new ImportError
                    {
                        RowNumber = row,
                        Field = "General",
                        ErrorMessage = ex.Message,
                        RowData = GetRowDataAsString(worksheet, row, headers)
                    });
                }
            }

            // Enviar correos de bienvenida masivos a los nuevos clientes
            if (newCustomers.Any())
            {
                try
                {
                    await _notificationService.SendBulkWelcomeEmailsAsync(newCustomers, cancellationToken);
                }
                catch (Exception)
                {
                  
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            result.Errors.Add(new ImportError
            {
                RowNumber = 0,
                Field = "General",
                ErrorMessage = $"Error al procesar el archivo: {ex.Message}"
            });
            return result;
        }
    }

    private Dictionary<string, int> ReadHeaders(ExcelWorksheet worksheet)
    {
        var headers = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        
        for (int col = 1; col <= worksheet.Dimension.Columns; col++)
        {
            var headerValue = worksheet.Cells[1, col].Value?.ToString()?.Trim();
            if (!string.IsNullOrEmpty(headerValue))
            {
                headers[headerValue] = col;
            }
        }
        
        return headers;
    }

    private Dictionary<string, object?> ReadRowData(ExcelWorksheet worksheet, int row, Dictionary<string, int> headers)
    {
        var data = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        
        foreach (var header in headers)
        {
            data[header.Key] = worksheet.Cells[row, header.Value].Value;
        }
        
        return data;
    }

    private string IdentifyDataType(Dictionary<string, int> headers)
    {
        var hasVehicleFields = headers.Keys.Any(h => 
            h.Contains("Brand", StringComparison.OrdinalIgnoreCase) ||
            h.Contains("Marca", StringComparison.OrdinalIgnoreCase) ||
            h.Contains("License Plate", StringComparison.OrdinalIgnoreCase) ||
            h.Contains("Placa", StringComparison.OrdinalIgnoreCase) ||
            h.Contains("Vehicle Type", StringComparison.OrdinalIgnoreCase) ||
            (h.Contains("Model", StringComparison.OrdinalIgnoreCase) && 
             h.Contains("Year", StringComparison.OrdinalIgnoreCase)));

        var hasProductFields = headers.Keys.Any(h => 
            h.Contains("SKU", StringComparison.OrdinalIgnoreCase) ||
            h.Contains("Producto", StringComparison.OrdinalIgnoreCase) ||
            h.Contains("Product", StringComparison.OrdinalIgnoreCase) ||
            h.Contains("Precio", StringComparison.OrdinalIgnoreCase) ||
            h.Contains("Price", StringComparison.OrdinalIgnoreCase) ||
            h.Contains("Stock", StringComparison.OrdinalIgnoreCase));

        var hasCustomerFields = headers.Keys.Any(h => 
            h.Contains("Cliente", StringComparison.OrdinalIgnoreCase) ||
            h.Contains("Customer", StringComparison.OrdinalIgnoreCase) ||
            h.Contains("Nombre", StringComparison.OrdinalIgnoreCase) ||
            h.Contains("Email", StringComparison.OrdinalIgnoreCase));

        var hasSaleFields = headers.Keys.Any(h => 
            h.Contains("Cantidad", StringComparison.OrdinalIgnoreCase) ||
            h.Contains("Quantity", StringComparison.OrdinalIgnoreCase));

        if (hasVehicleFields)
            return "vehicle";
            
        if (hasProductFields && hasCustomerFields && hasSaleFields)
            return "mixed";
        
        if (hasProductFields)
            return "product";
        
        if (hasCustomerFields)
            return "customer";

        return "mixed";
    }

    private async Task ProcessRow(Dictionary<string, object?> rowData, 
        string dataType, BulkImportResult result, int rowNumber,
        List<Product> existingProducts, List<Customer> existingCustomers, List<Customer> newCustomers)
    {
        if (dataType == "vehicle")
        {
            await ProcessVehicleRow(rowData, result, rowNumber);
        }
        else if (dataType == "mixed")
        {
            await ProcessMixedRow(rowData, result, rowNumber, existingProducts, existingCustomers, newCustomers);
        }
        else if (dataType == "product")
        {
            await ProcessProductRow(rowData, result, existingProducts);
        }
        else if (dataType == "customer")
        {
            await ProcessCustomerRow(rowData, result, existingCustomers, newCustomers);
        }
    }

    private async Task ProcessMixedRow(Dictionary<string, object?> rowData, BulkImportResult result, int rowNumber,
        List<Product> existingProducts, List<Customer> existingCustomers, List<Customer> newCustomers)
    {
        var productSku = GetStringValue(rowData, "SKU", "ProductSKU", "ProductoSKU", "Codigo", "Code");
        var productName = GetStringValue(rowData, "Product", "ProductName", "Producto", "NombreProducto", "Name");
        
        Guid? productId = null;
        
        // Procesar Producto si hay datos
        if (!string.IsNullOrWhiteSpace(productName))
        {
            try
            {
                var price = GetDecimalValue(rowData, "Price", "Precio", "ProductPrice", "PrecioProducto", "PrecioUnitario", "UnitPrice");
                var stock = GetDecimalValue(rowData, "Stock", "Existencia", "StockProducto", "Inventario", "Inventory");
                var description = GetStringValue(rowData, "Description", "Descripcion", "DescripcionProducto", "Desc");
                var imageUrl = GetStringValue(rowData, "ImageUrl", "Image", "Imagen", "Foto", "Photo");

                if (price <= 0)
                {
                    result.Errors.Add(new ImportError
                    {
                        RowNumber = rowNumber,
                        Field = "Precio",
                        ErrorMessage = "El precio del producto debe ser mayor a cero"
                    });
                }
                else
                {
                    var existingProduct = existingProducts.FirstOrDefault(p => 
                        !string.IsNullOrEmpty(productSku) && p.SKU.Equals(productSku, StringComparison.OrdinalIgnoreCase));

                    if (existingProduct != null)
                    {
                        existingProduct.Name = productName;
                        existingProduct.Price = price;
                        existingProduct.Stock = stock;
                        existingProduct.Description = description;
                        existingProduct.ImageUrl = imageUrl;
                        
                        await _productRepo.UpdateAsync(existingProduct);
                        result.ProductsUpdated++;
                        productId = existingProduct.Id;
                    }
                    else
                    {
                        var product = new Product(
                            productSku ?? $"SKU-{Guid.NewGuid().ToString()[..8]}",
                            productName,
                            description,
                            price,
                            imageUrl,
                            stock
                        );
                        
                        await _productRepo.AddAsync(product);
                        existingProducts.Add(product);
                        result.ProductsCreated++;
                        productId = product.Id;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add(new ImportError
                {
                    RowNumber = rowNumber,
                    Field = "Producto",
                    ErrorMessage = $"Error al procesar producto: {ex.Message}"
                });
            }
        }

        // Procesar Cliente si hay datos
        var customerEmail = GetStringValue(rowData, "Email", "ClienteEmail", "CustomerEmail", "Correo", "Mail");
        var firstName = GetStringValue(rowData, "Nombre", "FirstName", "Name");
        var lastName = GetStringValue(rowData, "Apellido", "LastName", "Surname");
        
        Guid? customerId = null;
        
        if (!string.IsNullOrWhiteSpace(customerEmail))
        {
            try
            {
                var document = GetStringValue(rowData, "Documento", "Document", "DNI", "ID");
                var phone = GetStringValue(rowData, "Telefono", "Phone", "Tel", "Celular");
                var address = GetStringValue(rowData, "Direccion", "Address", "Dir");

                if (string.IsNullOrWhiteSpace(firstName))
                    firstName = customerEmail.Split('@')[0];

                var existingCustomer = existingCustomers.FirstOrDefault(c => 
                    c.Email.Equals(customerEmail, StringComparison.OrdinalIgnoreCase));

                if (existingCustomer != null)
                {
                    existingCustomer.FirstName = firstName;
                    existingCustomer.LastName = lastName;
                    existingCustomer.Document = document ?? existingCustomer.Document;
                    existingCustomer.Phone = phone ?? existingCustomer.Phone;
                    existingCustomer.Address = address ?? existingCustomer.Address;
                    
                    await _customerRepo.UpdateAsync(existingCustomer);
                    result.CustomersUpdated++;
                    customerId = existingCustomer.Id;
                }
                else
                {
                    var customer = new Customer(firstName, lastName ?? string.Empty, customerEmail)
                    {
                        Document = document,
                        Phone = phone,
                        Address = address,
                        IsActive = true
                    };
                    
                    await _customerRepo.AddAsync(customer);
                    existingCustomers.Add(customer);
                    newCustomers.Add(customer);
                    result.CustomersCreated++;
                    customerId = customer.Id;
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add(new ImportError
                {
                    RowNumber = rowNumber,
                    Field = "Cliente",
                    ErrorMessage = $"Error al procesar cliente: {ex.Message}"
                });
            }
        }

        // Procesar Venta si hay datos suficientes
        var quantity = GetIntValue(rowData, "Quantity", "Cantidad", "Qty", "CantidadVendida", "CantidadVenta");
        
        if (quantity > 0 && productId.HasValue && customerId.HasValue)
        {
            try
            {
                var unitPrice = GetDecimalValue(rowData, "UnitPrice", "PrecioUnitario", "Price", "PrecioVenta");
                
                if (unitPrice <= 0)
                {
                    var product = existingProducts.FirstOrDefault(p => p.Id == productId.Value);
                    unitPrice = product?.Price ?? 0;
                }

                if (unitPrice <= 0)
                {
                    result.Errors.Add(new ImportError
                    {
                        RowNumber = rowNumber,
                        Field = "Venta",
                        ErrorMessage = "No se pudo determinar el precio unitario para la venta"
                    });
                }
                else
                {
                    var sale = new Sale(customerId.Value)
                    {
                        TotalAmount = unitPrice * quantity
                    };
                    
                    await _saleRepo.AddAsync(sale);

                    var saleItem = new SaleItem(productId.Value, quantity, unitPrice);
                    saleItem.AssignToSale(sale.Id);
                    
                    await _saleItemRepo.AddAsync(saleItem);
                    
                    result.SalesCreated++;
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add(new ImportError
                {
                    RowNumber = rowNumber,
                    Field = "Venta",
                    ErrorMessage = $"Error al procesar venta: {ex.Message}"
                });
            }
        }
    }

    private async Task ProcessProductRow(Dictionary<string, object?> rowData, BulkImportResult result, List<Product> existingProducts)
    {
        var sku = GetStringValue(rowData, "SKU", "Codigo", "Code");
        var name = GetStringValue(rowData, "Nombre", "Name", "Producto", "Product");
        var description = GetStringValue(rowData, "Descripcion", "Description", "Desc");
        var price = GetDecimalValue(rowData, "Precio", "Price", "PrecioUnitario", "UnitPrice");
        var stock = GetDecimalValue(rowData, "Stock", "Existencia", "Cantidad", "Inventory");
        var imageUrl = GetStringValue(rowData, "Imagen", "Image", "ImageUrl", "Foto");

        if (string.IsNullOrWhiteSpace(name))
            throw new Exception("El nombre del producto es obligatorio");
        
        if (price <= 0)
            throw new Exception("El precio debe ser mayor a cero");

        var existingProduct = existingProducts.FirstOrDefault(p => 
            !string.IsNullOrEmpty(sku) && p.SKU.Equals(sku, StringComparison.OrdinalIgnoreCase));

        if (existingProduct != null)
        {
            existingProduct.Name = name;
            existingProduct.Description = description;
            existingProduct.Price = price;
            existingProduct.Stock = stock;
            existingProduct.ImageUrl = imageUrl;
            
            await _productRepo.UpdateAsync(existingProduct);
            result.ProductsUpdated++;
        }
        else
        {
            var product = new Product(
                sku ?? $"SKU-{Guid.NewGuid().ToString()[..8]}",
                name,
                description,
                price,
                imageUrl,
                stock
            );
            
            await _productRepo.AddAsync(product);
            existingProducts.Add(product);
            result.ProductsCreated++;
        }
    }

    private async Task ProcessCustomerRow(Dictionary<string, object?> rowData, BulkImportResult result, List<Customer> existingCustomers, List<Customer> newCustomers)
    {
        var firstName = GetStringValue(rowData, "FirstName", "Nombre", "Name");
        var lastName = GetStringValue(rowData, "LastName", "Apellido", "Surname");
        var email = GetStringValue(rowData, "Email", "Correo", "Mail");
        var document = GetStringValue(rowData, "Document", "Documento", "DNI", "ID");
        var phone = GetStringValue(rowData, "Phone", "Telefono", "Tel", "Celular");
        var address = GetStringValue(rowData, "Address", "Direccion", "Dir");

        if (string.IsNullOrWhiteSpace(firstName))
            throw new Exception("El nombre es obligatorio");
        
        if (string.IsNullOrWhiteSpace(email))
            throw new Exception("El email es obligatorio");

        var existingCustomer = existingCustomers.FirstOrDefault(c => 
            c.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

        if (existingCustomer != null)
        {
            existingCustomer.FirstName = firstName;
            existingCustomer.LastName = lastName;
            existingCustomer.Email = email;
            existingCustomer.Document = document;
            existingCustomer.Phone = phone;
            existingCustomer.Address = address;
            
            await _customerRepo.UpdateAsync(existingCustomer);
            result.CustomersUpdated++;
        }
        else
        {
            var customer = new Customer(firstName, lastName ?? string.Empty, email)
            {
                Document = document,
                Phone = phone,
                Address = address,
                IsActive = true
            };
            
            await _customerRepo.AddAsync(customer);
            existingCustomers.Add(customer);
            newCustomers.Add(customer);
            result.CustomersCreated++;
        }
    }

    private async Task ProcessVehicleRow(Dictionary<string, object?> rowData, BulkImportResult result, int rowNumber)
    {
        try
        {
            var brand = GetStringValue(rowData, "Brand", "Marca");
            var model = GetStringValue(rowData, "Model", "Modelo");
            var licensePlate = GetStringValue(rowData, "License Plate", "Placa", "LicensePlate");
            var vehicleTypeStr = GetStringValue(rowData, "Vehicle Type", "VehicleType", "Tipo", "Type");
            var year = GetIntValue(rowData, "Year", "Año", "Anno");

            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(brand))
            {
                result.Errors.Add(new ImportError
                {
                    RowNumber = rowNumber,
                    Field = "Brand",
                    ErrorMessage = "Brand is required"
                });
                return;
            }

            if (string.IsNullOrWhiteSpace(model))
            {
                result.Errors.Add(new ImportError
                {
                    RowNumber = rowNumber,
                    Field = "Model",
                    ErrorMessage = "Model is required"
                });
                return;
            }

            if (string.IsNullOrWhiteSpace(licensePlate))
            {
                result.Errors.Add(new ImportError
                {
                    RowNumber = rowNumber,
                    Field = "License Plate",
                    ErrorMessage = "License Plate is required"
                });
                return;
            }

            if (year < 1900 || year > DateTime.Now.Year + 1)
            {
                result.Errors.Add(new ImportError
                {
                    RowNumber = rowNumber,
                    Field = "Year",
                    ErrorMessage = $"Invalid year: {year}"
                });
                return;
            }

            // Parse VehicleType
            if (!Enum.TryParse<Domain.Enums.VehicleType>(vehicleTypeStr, true, out var vehicleType))
            {
                result.Errors.Add(new ImportError
                {
                    RowNumber = rowNumber,
                    Field = "Vehicle Type",
                    ErrorMessage = $"Invalid vehicle type: {vehicleTypeStr}. Valid types: Excavator, Forklift, DumpTruck, Crane, Backhoe, FrontLoader, etc."
                });
                return;
            }

            // Leer otros campos
            var serialNumber = GetStringValue(rowData, "Serial Number", "SerialNumber", "NumeroSerie");
            var hourlyRate = GetDecimalValue(rowData, "Hourly Rate", "HourlyRate", "TarifaPorHora");
            var dailyRate = GetDecimalValue(rowData, "Daily Rate", "DailyRate", "TarifaDiaria");
            var weeklyRate = GetDecimalValue(rowData, "Weekly Rate", "WeeklyRate", "TarifaSemanal");
            var monthlyRate = GetDecimalValue(rowData, "Monthly Rate", "MonthlyRate", "TarifaMensual");
            var currentHours = GetDecimalValue(rowData, "Current Hours", "CurrentHours", "HorasActuales");
            var currentMileage = GetDecimalValue(rowData, "Current Mileage", "CurrentMileage", "KilometrajeActual");
            var maintenanceInterval = GetDecimalValue(rowData, "Maintenance Hours Interval", "MaintenanceHoursInterval", "IntervaloMantenimiento");
            var specifications = GetStringValue(rowData, "Specifications", "Especificaciones");
            var imageUrl = GetStringValue(rowData, "Image URL", "ImageUrl", "ImagenUrl");
            var documentsUrl = GetStringValue(rowData, "Documents URL", "DocumentsUrl");
            var notes = GetStringValue(rowData, "Notes", "Notas");
            var isActiveStr = GetStringValue(rowData, "Is Active", "IsActive", "Activo");
            var isActive = string.IsNullOrWhiteSpace(isActiveStr) || 
                           isActiveStr.Equals("Yes", StringComparison.OrdinalIgnoreCase) ||
                           isActiveStr.Equals("Si", StringComparison.OrdinalIgnoreCase) ||
                           isActiveStr.Equals("True", StringComparison.OrdinalIgnoreCase);

            // Validar que al menos tenga una tarifa
            if (hourlyRate <= 0 && dailyRate <= 0 && weeklyRate <= 0 && monthlyRate <= 0)
            {
                result.Errors.Add(new ImportError
                {
                    RowNumber = rowNumber,
                    Field = "Rates",
                    ErrorMessage = "At least one rate (Hourly, Daily, Weekly, or Monthly) must be greater than 0"
                });
                return;
            }

            // Buscar si existe el vehículo por placa
            var existingVehicles = await _vehicleRepo.GetAllAsync();
            var existingVehicle = existingVehicles.FirstOrDefault(v => 
                v.LicensePlate.Equals(licensePlate, StringComparison.OrdinalIgnoreCase));

            if (existingVehicle != null)
            {
                // Actualizar vehículo existente
                existingVehicle.Brand = brand;
                existingVehicle.Model = model;
                existingVehicle.Year = year;
                existingVehicle.VehicleType = vehicleType;
                existingVehicle.SerialNumber = serialNumber;
                existingVehicle.HourlyRate = hourlyRate;
                existingVehicle.DailyRate = dailyRate;
                existingVehicle.WeeklyRate = weeklyRate;
                existingVehicle.MonthlyRate = monthlyRate;
                existingVehicle.CurrentHours = currentHours;
                existingVehicle.CurrentMileage = currentMileage;
                existingVehicle.MaintenanceHoursInterval = maintenanceInterval;
                existingVehicle.Specifications = specifications;
                existingVehicle.ImageUrl = imageUrl;
                existingVehicle.DocumentsUrl = documentsUrl;
                existingVehicle.Notes = notes;
                existingVehicle.IsActive = isActive;
                existingVehicle.UpdatedAt = DateTime.UtcNow;

                await _vehicleRepo.UpdateAsync(existingVehicle);
                result.ProductsUpdated++; // Usamos el contador de productos para vehículos por ahora
            }
            else
            {
                // Crear nuevo vehículo usando el constructor público
                var newVehicle = new Vehicle(brand, model, year, licensePlate, vehicleType)
                {
                    SerialNumber = serialNumber,
                    HourlyRate = hourlyRate,
                    DailyRate = dailyRate,
                    WeeklyRate = weeklyRate,
                    MonthlyRate = monthlyRate,
                    CurrentHours = currentHours,
                    CurrentMileage = currentMileage,
                    MaintenanceHoursInterval = maintenanceInterval,
                    Specifications = specifications,
                    ImageUrl = imageUrl,
                    DocumentsUrl = documentsUrl,
                    Notes = notes,
                    Status = Domain.Enums.VehicleStatus.Available,
                    IsActive = isActive,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _vehicleRepo.AddAsync(newVehicle);
                result.ProductsCreated++; // Usamos el contador de productos para vehículos por ahora
            }
        }
        catch (Exception ex)
        {
            result.Errors.Add(new ImportError
            {
                RowNumber = rowNumber,
                Field = "General",
                ErrorMessage = $"Error processing vehicle: {ex.Message}"
            });
        }
    }

    private bool IsEmptyRow(Dictionary<string, object?> rowData)
    {
        return rowData.All(kvp => kvp.Value == null || string.IsNullOrWhiteSpace(kvp.Value.ToString()));
    }

    private string GetStringValue(Dictionary<string, object?> data, params string[] possibleKeys)
    {
        foreach (var key in possibleKeys)
        {
            if (data.TryGetValue(key, out var value) && value != null)
            {
                return value.ToString()?.Trim() ?? string.Empty;
            }
        }
        return string.Empty;
    }

    private decimal GetDecimalValue(Dictionary<string, object?> data, params string[] possibleKeys)
    {
        foreach (var key in possibleKeys)
        {
            if (data.TryGetValue(key, out var value) && value != null)
            {
                if (decimal.TryParse(value.ToString(), out var result))
                    return result;
            }
        }
        return 0;
    }

    private int GetIntValue(Dictionary<string, object?> data, params string[] possibleKeys)
    {
        foreach (var key in possibleKeys)
        {
            if (data.TryGetValue(key, out var value) && value != null)
            {
                if (int.TryParse(value.ToString(), out var result))
                    return result;
            }
        }
        return 0;
    }

    private string GetRowDataAsString(ExcelWorksheet worksheet, int row, Dictionary<string, int> headers)
    {
        var values = new List<string>();
        foreach (var header in headers.OrderBy(h => h.Value).Take(5))
        {
            var value = worksheet.Cells[row, header.Value].Value?.ToString() ?? "";
            if (!string.IsNullOrEmpty(value))
                values.Add($"{header.Key}: {value}");
        }
        return string.Join(" | ", values);
    }
}
