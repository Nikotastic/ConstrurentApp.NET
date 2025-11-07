# 📊 Sistema de Importación Masiva de Datos Desnormalizados

## ✅ Implementación Completada

Se ha implementado exitosamente un sistema completo de carga masiva de datos mediante archivos Excel (.xlsx) con las siguientes características:

### 🎯 Características Implementadas

#### 1. **Normalización Automática**
- El sistema identifica automáticamente qué columnas pertenecen a cada tabla
- Separa datos de Productos, Clientes y Ventas aunque estén mezclados en una sola fila
- No requiere archivos separados por entidad

#### 2. **Detección Flexible de Columnas**
El sistema reconoce múltiples variantes de nombres de columnas:

**Productos:**
- SKU, Codigo, Code, ProductSKU
- Producto, Product, ProductName, NombreProducto
- Descripcion, Description, DescripcionProducto
- Precio, Price, PrecioUnitario, UnitPrice
- Stock, Existencia, Inventario, Cantidad

**Clientes:**
- Nombre, FirstName, Name
- Apellido, LastName, Surname
- Email, Correo, Mail, CustomerEmail
- Telefono, Phone, Tel, Celular
- Direccion, Address, Dir
- Documento, Document, DNI, ID

**Ventas:**
- Cantidad, Quantity, Qty, CantidadVendida
- Fecha, Date, FechaVenta, SaleDate

#### 3. **Validaciones**
- ✅ Nombre de producto (obligatorio)
- ✅ Precio de producto > 0 (obligatorio)
- ✅ Nombre de cliente (obligatorio)
- ✅ Email de cliente (obligatorio y único)
- ✅ Cantidad de venta > 0 (obligatorio)

#### 4. **Inserción/Actualización Inteligente**
- **Productos:** Busca por SKU, actualiza si existe o crea nuevo
- **Clientes:** Busca por Email, actualiza si existe o crea nuevo
- **Ventas:** Siempre crea nuevas ventas con sus items

#### 5. **Log Detallado de Errores**
- Número de fila con error
- Campo que causó el problema
- Mensaje descriptivo del error
- Datos de la fila para referencia

### 📁 Archivos Creados

```
Firmness.Application/
├── Models/
│   └── BulkImportResult.cs         # Modelo de resultado con estadísticas
├── Interfaces/
│   └── IBulkImportService.cs       # Interfaz del servicio
└── Services/
    └── BulkImportService.cs        # Lógica de importación

Firmness.Admin.Web/
├── Controllers/
│   └── BulkImportController.cs     # Controlador MVC
└── Views/
    └── BulkImport/
        ├── Index.cshtml            # Vista de carga
        └── Result.cshtml           # Vista de resultados

Firmness.Core/
└── Interfaces/
    ├── IProductRepository.cs       # +GetBySKUAsync()
    └── ICustomerRepository.cs      # +GetByEmailAsync()

Firmness.Infrastructure/
├── Repositories/
│   ├── ProductRepository.cs        # Implementación GetBySKUAsync()
│   └── CustomerRepository.cs       # Implementación GetByEmailAsync()
└── DependencyInjection/
    └── ServiceCollectionExtensions.cs  # Registro del servicio
```

### 🚀 Cómo Usar

#### 1. Acceder a la funcionalidad
Navega a: `https://localhost:XXXX/BulkImport`

#### 2. Preparar archivo Excel
Crea un archivo .xlsx con los siguientes encabezados (puedes usar todos o solo algunos):

```
SKU | Producto | Descripcion | Precio | Stock | Nombre | Apellido | Email | Telefono | Direccion | Documento | Cantidad | Fecha
```

#### 3. Ejemplos de Datos

**Ejemplo 1: Producto + Cliente + Venta (todo en una fila)**
```
PROD001 | Laptop HP | Laptop i7 16GB | 850.00 | 10 | Juan | Pérez | juan@email.com | 555-1234 | Calle 123 | 12345678A | 2 | 2025-11-07
```

**Ejemplo 2: Solo Producto (inventario)**
```
PROD002 | Mouse Logitech | Mouse inalámbrico | 99.99 | 25 | | | | | | | |
```

**Ejemplo 3: Solo Cliente**
```
| | | | | Pedro | Sánchez | pedro@email.com | 555-7890 | Av. Central 654 | 99887766E | |
```

**Ejemplo 4: Venta de productos/clientes existentes**
```
PROD001 | | | | | Juan | Pérez | juan@email.com | | | | 3 | 2025-11-07
```

### 📋 Estructura del Archivo Excel de Prueba

He creado un archivo de ejemplo con 10 filas que incluye:

- **7 Productos diferentes** (PROD001 a PROD007)
- **7 Clientes diferentes**
- **8 Ventas**
- **Casos de prueba:**
  - Datos completamente mezclados
  - Clientes repetidos (para probar actualización)
  - Productos sin ventas (solo inventario)
  - Clientes sin ventas (solo registro)
  - Ventas cruzadas (cliente existente + producto existente)

### 🎨 Interfaz de Usuario

#### Pantalla de Carga (`/BulkImport`)
- Formulario de carga de archivos
- Validación de extensión (.xlsx, .xls)
- Validación de tamaño (máx. 10MB)
- Instrucciones detalladas
- Botón para descargar plantilla de ejemplo

#### Pantalla de Resultados (`/BulkImport/Result`)
- Resumen estadístico:
  - Total de filas procesadas
  - Filas exitosas vs fallidas
  - Tasa de éxito
- Desglose por entidad:
  - Productos: creados/actualizados
  - Clientes: creados/actualizados
  - Ventas: creadas
- Tabla detallada de errores (si los hay)
- Botones de navegación rápida

### 📊 Resultado Esperado

Después de importar el archivo de ejemplo, deberías ver:

```
✅ Importación Exitosa
────────────────────────
Total de Filas: 10
Exitosas: 8-9
Fallidas: 0-2

📦 Productos
- Creados: 7
- Actualizados: 0

👥 Clientes  
- Creados: 7
- Actualizados: 1-2

🛒 Ventas
- Creadas: 8
```

### 🔧 Cómo Crear tu Archivo Excel de Prueba

#### Opción 1: Manualmente en Excel
1. Abre Microsoft Excel
2. En la primera fila, escribe los encabezados
3. Rellena las filas con datos de ejemplo
4. Guarda como `.xlsx`

#### Opción 2: Usar el generador automático
```bash
cd C:\Users\NikoC\RiderProjects\ConstrurentApp.NET\scripts\ExcelGenerator
dotnet run
```

Esto creará `Datos_Prueba_Importacion.xlsx` en el directorio actual.

### ⚠️ Notas Importantes

1. **Primera fila = Encabezados:** La primera fila siempre debe contener los nombres de las columnas
2. **Emails únicos:** El email es la clave única para clientes
3. **SKU único:** El SKU es la clave única para productos
4. **Filas vacías:** Se ignoran automáticamente
5. **Orden de columnas:** No importa el orden, el sistema las detecta por nombre
6. **Columnas faltantes:** Si no tienes datos para alguna columna, puedes omitirla

### 🐛 Manejo de Errores

El sistema genera logs detallados para cada error:

```
Fila 8 | Campo: ProductPrice | Error: El precio debe ser mayor a cero
Fila 12 | Campo: CustomerEmail | Error: El email del cliente es obligatorio
```

### 📝 Próximos Pasos

1. **Ejecuta la aplicación:**
   ```bash
   cd Firmness.Admin.Web
   dotnet run
   ```

2. **Navega a:**
   ```
   https://localhost:XXXX/BulkImport
   ```

3. **Crea un archivo Excel** con los datos de ejemplo proporcionados

4. **Carga el archivo** y observa los resultados

### ✨ Ventajas del Sistema

- ✅ **Ahorro de tiempo:** Carga cientos de registros en segundos
- ✅ **Flexible:** Acepta datos en cualquier orden
- ✅ **Inteligente:** Detecta y normaliza automáticamente
- ✅ **Robusto:** Maneja errores sin detener el proceso
- ✅ **Informativo:** Logs detallados de cada operación
- ✅ **Actualización automática:** No duplica registros existentes

---

**¡El sistema está listo para usar! 🚀**

