using Firmness.Application.Interfaces;
using Firmness.Application.Models;
using Firmness.Core.Entities;
using Firmness.Core.Interfaces;
using OfficeOpenXml;

namespace Firmness.Application.Services;

public class BulkImportService : IBulkImportService
{
    private readonly IProductRepository _productRepo;
    private readonly ICustomerRepository _customerRepo;
    private readonly ISaleRepository _saleRepo;
    private readonly ISaleItemRepository _saleItemRepo;

    public BulkImportService(
        IProductRepository productRepo,
        ICustomerRepository customerRepo,
        ISaleRepository saleRepo,
        ISaleItemRepository saleItemRepo)
    {
        _productRepo = productRepo;
        _customerRepo = customerRepo;
        _saleRepo = saleRepo;
        _saleItemRepo = saleItemRepo;
    }

    public async Task<BulkImportResult> ImportFromExcelAsync(Stream fileStream, CancellationToken cancellationToken = default)
    {
        var result = new BulkImportResult();

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

            for (int row = 2; row <= worksheet.Dimension.Rows; row++)
            {
                try
                {
                    var rowData = ReadRowData(worksheet, row, headers);
                    
                    if (IsEmptyRow(rowData))
                        continue;

                    await ProcessRow(rowData, headers, dataType, result, row);
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

        if (hasProductFields && hasCustomerFields && hasSaleFields)
            return "mixed";
        
        if (hasProductFields)
            return "product";
        
        if (hasCustomerFields)
            return "customer";

        return "mixed";
    }

    private async Task ProcessRow(Dictionary<string, object?> rowData, Dictionary<string, int> headers, 
        string dataType, BulkImportResult result, int rowNumber)
    {
        if (dataType == "mixed")
        {
            await ProcessMixedRow(rowData, result, rowNumber);
        }
        else if (dataType == "product")
        {
            await ProcessProductRow(rowData, result);
        }
        else if (dataType == "customer")
        {
            await ProcessCustomerRow(rowData, result);
        }
    }

    private async Task ProcessMixedRow(Dictionary<string, object?> rowData, BulkImportResult result, int rowNumber)
    {
        var productSku = GetStringValue(rowData, "SKU", "ProductoSKU", "ProductSKU");
        var productName = GetStringValue(rowData, "Producto", "ProductName", "NombreProducto", "Product");
        
        Guid? productId = null;
        
        if (!string.IsNullOrWhiteSpace(productName))
        {
            try
            {
                var price = GetDecimalValue(rowData, "Precio", "Price", "PrecioProducto", "ProductPrice");
                var stock = GetDecimalValue(rowData, "Stock", "Existencia", "StockProducto");
                var description = GetStringValue(rowData, "Descripcion", "Description", "DescripcionProducto");
                var imageUrl = GetStringValue(rowData, "Imagen", "Image", "ImageUrl");

                if (price <= 0)
                    throw new Exception("El precio del producto debe ser mayor a cero");

                var existingProducts = await _productRepo.GetAllAsync();
                var existingProduct = existingProducts.FirstOrDefault(p => 
                    !string.IsNullOrEmpty(productSku) && p.SKU.Equals(productSku, StringComparison.OrdinalIgnoreCase));

                if (existingProduct != null)
                {
                    existingProduct.Name = productName;
                    existingProduct.Price = price;
                    existingProduct.Stock = stock;
                    existingProduct.Description = description ?? string.Empty;
                    existingProduct.ImageUrl = imageUrl ?? string.Empty;
                    
                    await _productRepo.UpdateAsync(existingProduct);
                    result.ProductsUpdated++;
                    productId = existingProduct.Id;
                }
                else
                {
                    var product = new Product(
                        productSku ?? $"SKU-{Guid.NewGuid().ToString()[..8]}",
                        productName,
                        description ?? string.Empty,
                        price,
                        imageUrl ?? string.Empty,
                        stock
                    );
                    
                    await _productRepo.AddAsync(product);
                    result.ProductsCreated++;
                    productId = product.Id;
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

        var customerEmail = GetStringValue(rowData, "Email", "ClienteEmail", "CustomerEmail", "Correo");
        var customerName = GetStringValue(rowData, "Cliente", "CustomerName", "NombreCliente", "Customer");
        
        Guid? customerId = null;
        
        if (!string.IsNullOrWhiteSpace(customerEmail))
        {
            try
            {
                var firstName = customerName ?? GetStringValue(rowData, "Nombre", "FirstName");
                var lastName = GetStringValue(rowData, "Apellido", "LastName", "Surname");
                var document = GetStringValue(rowData, "Documento", "Document", "DNI");
                var phone = GetStringValue(rowData, "Telefono", "Phone", "Tel");
                var address = GetStringValue(rowData, "Direccion", "Address", "Dir");

                if (string.IsNullOrWhiteSpace(firstName))
                    firstName = customerEmail.Split('@')[0];

                var existingCustomers = await _customerRepo.GetAllAsync();
                var existingCustomer = existingCustomers.FirstOrDefault(c => 
                    c.Email.Equals(customerEmail, StringComparison.OrdinalIgnoreCase));

                if (existingCustomer != null)
                {
                    existingCustomer.FirstName = firstName;
                    existingCustomer.LastName = lastName ?? string.Empty;
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
                        Document = document ?? string.Empty,
                        Phone = phone ?? string.Empty,
                        Address = address ?? string.Empty,
                        IsActive = true
                    };
                    
                    await _customerRepo.AddAsync(customer);
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

        var quantity = GetIntValue(rowData, "Cantidad", "Quantity", "Qty", "CantidadVenta");
        
        if (quantity > 0 && productId.HasValue && customerId.HasValue)
        {
            try
            {
                var unitPrice = GetDecimalValue(rowData, "PrecioUnitario", "UnitPrice", "PrecioVenta");
                
                if (unitPrice <= 0)
                {
                    var product = await _productRepo.GetByIdAsync(productId.Value);
                    unitPrice = product?.Price ?? 0;
                }

                if (unitPrice <= 0)
                    throw new Exception("No se pudo determinar el precio unitario para la venta");

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

    private async Task ProcessProductRow(Dictionary<string, object?> rowData, BulkImportResult result)
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

        var existingProducts = await _productRepo.GetAllAsync();
        var existingProduct = existingProducts.FirstOrDefault(p => 
            !string.IsNullOrEmpty(sku) && p.SKU.Equals(sku, StringComparison.OrdinalIgnoreCase));

        if (existingProduct != null)
        {
            existingProduct.Name = name;
            existingProduct.Description = description ?? string.Empty;
            existingProduct.Price = price;
            existingProduct.Stock = stock;
            existingProduct.ImageUrl = imageUrl ?? string.Empty;
            
            await _productRepo.UpdateAsync(existingProduct);
            result.ProductsUpdated++;
        }
        else
        {
            var product = new Product(
                sku ?? $"SKU-{Guid.NewGuid().ToString()[..8]}",
                name,
                description ?? string.Empty,
                price,
                imageUrl ?? string.Empty,
                stock
            );
            
            await _productRepo.AddAsync(product);
            result.ProductsCreated++;
        }
    }

    private async Task ProcessCustomerRow(Dictionary<string, object?> rowData, BulkImportResult result)
    {
        var firstName = GetStringValue(rowData, "Nombre", "FirstName", "Name");
        var lastName = GetStringValue(rowData, "Apellido", "LastName", "Surname");
        var email = GetStringValue(rowData, "Email", "Correo", "Mail");
        var document = GetStringValue(rowData, "Documento", "Document", "DNI", "ID");
        var phone = GetStringValue(rowData, "Telefono", "Phone", "Tel", "Celular");
        var address = GetStringValue(rowData, "Direccion", "Address", "Dir");

        if (string.IsNullOrWhiteSpace(firstName))
            throw new Exception("El nombre es obligatorio");
        
        if (string.IsNullOrWhiteSpace(email))
            throw new Exception("El email es obligatorio");

        var existingCustomers = await _customerRepo.GetAllAsync();
        var existingCustomer = existingCustomers.FirstOrDefault(c => 
            c.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

        if (existingCustomer != null)
        {
            existingCustomer.FirstName = firstName;
            existingCustomer.LastName = lastName ?? string.Empty;
            existingCustomer.Email = email;
            existingCustomer.Document = document ?? string.Empty;
            existingCustomer.Phone = phone ?? string.Empty;
            existingCustomer.Address = address ?? string.Empty;
            
            await _customerRepo.UpdateAsync(existingCustomer);
            result.CustomersUpdated++;
        }
        else
        {
            var customer = new Customer(firstName, lastName ?? string.Empty, email)
            {
                Document = document ?? string.Empty,
                Phone = phone ?? string.Empty,
                Address = address ?? string.Empty,
                IsActive = true
            };
            
            await _customerRepo.AddAsync(customer);
            result.CustomersCreated++;
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
