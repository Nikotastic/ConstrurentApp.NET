using Firmness.Application.Interfaces;
using Firmness.Domain.Entities;
using Firmness.Domain.Enums;
using Firmness.Domain.Interfaces;
using Firmness.Web.ViewModels.Vehicle;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Firmness.Web.Controllers;

[Authorize]
public class VehiclesController : Controller
{
    private readonly IVehicleService _vehicleService;
    private readonly IVehicleRentalService _rentalService;
    private readonly IExportService _exportService;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<VehiclesController> _logger;

    public VehiclesController(
        IVehicleService vehicleService,
        IVehicleRentalService rentalService,
        IExportService exportService,
        IFileStorageService fileStorageService,
        ILogger<VehiclesController> logger)
    {
        _vehicleService = vehicleService;
        _rentalService = rentalService;
        _exportService = exportService;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    // GET: Vehicles
    public async Task<IActionResult> Index(int page = 1, int pageSize = 20, string? search = null, string? vehicleType = null, string? status = null)
    {
        // Prepare filter dropdowns - always set these
        ViewBag.VehicleTypes = Enum.GetNames(typeof(VehicleType))
            .Select(name => new SelectListItem { Value = name, Text = name })
            .ToList();

        ViewBag.Statuses = Enum.GetNames(typeof(VehicleStatus))
            .Select(name => new SelectListItem { Value = name, Text = name })
            .ToList();

        try
        {
            var result = await _vehicleService.GetAllVehiclesAsync();
            
            if (!result.IsSuccess || result.Value == null)
            {
                TempData["Error"] = "Could not load vehicles.";
                return View(new PaginatedVehicleListViewModel
                {
                    Vehicles = new List<VehicleListViewModel>(),
                    CurrentPage = 1,
                    PageSize = pageSize,
                    TotalItems = 0,
                    TotalPages = 0
                });
            }

            var vehicles = result.Value.Select(MapToListViewModel).ToList();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(search))
            {
                vehicles = vehicles.Where(v =>
                    v.Brand.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    v.Model.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    v.LicensePlate.Contains(search, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            if (!string.IsNullOrWhiteSpace(vehicleType))
            {
                vehicles = vehicles.Where(v => v.VehicleType == vehicleType).ToList();
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                vehicles = vehicles.Where(v => v.Status == status).ToList();
            }

            // Apply pagination
            var totalItems = vehicles.Count;
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            
            var paginatedVehicles = vehicles
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewModel = new PaginatedVehicleListViewModel
            {
                Vehicles = paginatedVehicles,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Search = search,
                VehicleType = vehicleType,
                Status = status
            };


            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading vehicles");
            TempData["Error"] = "An error occurred while loading vehicles.";
            return View(new PaginatedVehicleListViewModel
            {
                Vehicles = new List<VehicleListViewModel>(),
                CurrentPage = 1,
                PageSize = pageSize,
                TotalItems = 0,
                TotalPages = 0
            });
        }
    }

    // GET: Vehicles/Details/5
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var result = await _vehicleService.GetVehicleByIdAsync(id);
            if (!result.IsSuccess || result.Value == null)
            {
                TempData["Error"] = "Vehicle not found.";
                return RedirectToAction(nameof(Index));
            }

            // Get rental history
            var rentalsResult = await _rentalService.GetRentalsByVehicleIdAsync(id);
            var recentRentals = rentalsResult.IsSuccess && rentalsResult.Value != null
                ? rentalsResult.Value.OrderByDescending(r => r.StartDate).Take(10)
                    .Select(r => new VehicleRentalSummary
                    {
                        Id = r.Id,
                        CustomerName = r.CustomerName,
                        StartDate = r.StartDate,
                        EstimatedReturnDate = r.EstimatedReturnDate,
                        ActualReturnDate = r.ActualReturnDate,
                        Status = r.Status,
                        TotalAmount = r.TotalAmount
                    }).ToList()
                : new List<VehicleRentalSummary>();

            var viewModel = MapToDetailsViewModel(result.Value);
            viewModel.RecentRentals = recentRentals;
            viewModel.TotalRentals = rentalsResult.Value?.Count() ?? 0;
            viewModel.ActiveRentals = rentalsResult.Value?.Count(r => r.Status == "Active") ?? 0;

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading vehicle details for {VehicleId}", id);
            TempData["Error"] = "An error occurred while loading vehicle details.";
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: Vehicles/Create
    [Authorize(Roles = "Admin,Manager")]
    public IActionResult Create()
    {
        var viewModel = new VehicleFormViewModel
        {
            Year = DateTime.Now.Year,
            VehicleTypes = GetVehicleTypeSelectList(),
            Statuses = GetStatusSelectList(),
            Status = (int)VehicleStatus.Available
        };
        return View(viewModel);
    }

    // POST: Vehicles/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Create(VehicleFormViewModel viewModel, IFormFile? imageFile)
    {
        if (!ModelState.IsValid)
        {
            viewModel.VehicleTypes = GetVehicleTypeSelectList();
            return View(viewModel);
        }

        try
        {
            // Handle image upload
            string imageUrl = viewModel.ImageUrl ?? string.Empty;
            if (imageFile != null && imageFile.Length > 0)
            {
                _logger.LogInformation("Uploading vehicle image: {FileName}", imageFile.FileName);
                try
                {
                    imageUrl = await _fileStorageService.UploadFileAsync(
                        imageFile.FileName,
                        imageFile.OpenReadStream(),
                        imageFile.ContentType,
                        "vehicles"
                    );
                    _logger.LogInformation("Image uploaded successfully: {ImageUrl}", imageUrl);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error uploading vehicle image");
                    ModelState.AddModelError("imageFile", "Error al subir la imagen. Por favor intenta de nuevo.");
                    viewModel.VehicleTypes = GetVehicleTypeSelectList();
                    return View(viewModel);
                }
            }

            var createDto = new Domain.DTOs.Vehicle.CreateVehicleDto
            {
                Brand = viewModel.Brand,
                Model = viewModel.Model,
                Year = viewModel.Year,
                LicensePlate = viewModel.LicensePlate,
                VehicleType = (VehicleType)viewModel.VehicleType,
                HourlyRate = viewModel.HourlyRate,
                DailyRate = viewModel.DailyRate,
                WeeklyRate = viewModel.WeeklyRate,
                MonthlyRate = viewModel.MonthlyRate,
                CurrentHours = viewModel.CurrentHours,
                CurrentMileage = viewModel.CurrentMileage,
                Specifications = viewModel.Specifications,
                SerialNumber = viewModel.SerialNumber,
                MaintenanceHoursInterval = viewModel.MaintenanceHoursInterval,
                LastMaintenanceDate = viewModel.LastMaintenanceDate.HasValue 
                    ? DateTime.SpecifyKind(viewModel.LastMaintenanceDate.Value, DateTimeKind.Utc) 
                    : null,
                NextMaintenanceDate = viewModel.NextMaintenanceDate.HasValue 
                    ? DateTime.SpecifyKind(viewModel.NextMaintenanceDate.Value, DateTimeKind.Utc) 
                    : null,
                ImageUrl = imageUrl,
                DocumentsUrl = viewModel.DocumentsUrl,
                Notes = viewModel.Notes
            };

            var result = await _vehicleService.CreateVehicleAsync(createDto);

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.ErrorMessage;
                viewModel.VehicleTypes = GetVehicleTypeSelectList();
                return View(viewModel);
            }

            TempData["Success"] = $"Vehicle {result.Value?.DisplayName ?? "Unknown"} created successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating vehicle");
            TempData["Error"] = "An error occurred while creating the vehicle.";
            viewModel.VehicleTypes = GetVehicleTypeSelectList();
            viewModel.Statuses = GetStatusSelectList();
            return View(viewModel);
        }
    }

    // GET: Vehicles/Edit/5
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var result = await _vehicleService.GetVehicleByIdAsync(id);
            if (!result.IsSuccess || result.Value == null)
            {
                TempData["Error"] = "Vehicle not found.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = MapToFormViewModel(result.Value);
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading vehicle for edit {VehicleId}", id);
            TempData["Error"] = "An error occurred while loading the vehicle.";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Vehicles/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Edit(Guid id, VehicleFormViewModel viewModel, IFormFile? imageFile)
    {
        if (id != viewModel.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            viewModel.VehicleTypes = GetVehicleTypeSelectList();
            return View(viewModel);
        }

        try
        {
            // Handle image upload
            string imageUrl = viewModel.ImageUrl ?? string.Empty;
            if (imageFile != null && imageFile.Length > 0)
            {
                _logger.LogInformation("Uploading new vehicle image: {FileName}", imageFile.FileName);
                try
                {
                    // Delete old image if exists
                    if (!string.IsNullOrWhiteSpace(viewModel.ImageUrl))
                    {
                        try
                        {
                            await _fileStorageService.DeleteFileAsync(viewModel.ImageUrl);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Could not delete old vehicle image: {ImageUrl}", viewModel.ImageUrl);
                        }
                    }

                    // Upload new image
                    imageUrl = await _fileStorageService.UploadFileAsync(
                        imageFile.FileName,
                        imageFile.OpenReadStream(),
                        imageFile.ContentType,
                        "vehicles"
                    );
                    _logger.LogInformation("New image uploaded successfully: {ImageUrl}", imageUrl);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error uploading vehicle image");
                    ModelState.AddModelError("imageFile", "Error al subir la imagen. Por favor intenta de nuevo.");
                    viewModel.VehicleTypes = GetVehicleTypeSelectList();
                    return View(viewModel);
                }
            }

            var updateDto = new Domain.DTOs.Vehicle.UpdateVehicleDto
            {
                Brand = viewModel.Brand,
                Model = viewModel.Model,
                Year = viewModel.Year,
                LicensePlate = viewModel.LicensePlate,
                VehicleType = (VehicleType)viewModel.VehicleType,
                Status = (VehicleStatus)viewModel.Status,
                HourlyRate = viewModel.HourlyRate,
                DailyRate = viewModel.DailyRate,
                WeeklyRate = viewModel.WeeklyRate,
                MonthlyRate = viewModel.MonthlyRate,
                CurrentHours = viewModel.CurrentHours,
                CurrentMileage = viewModel.CurrentMileage,
                Specifications = viewModel.Specifications,
                SerialNumber = viewModel.SerialNumber,
                MaintenanceHoursInterval = viewModel.MaintenanceHoursInterval,
                LastMaintenanceDate = viewModel.LastMaintenanceDate.HasValue 
                    ? DateTime.SpecifyKind(viewModel.LastMaintenanceDate.Value, DateTimeKind.Utc) 
                    : null,
                NextMaintenanceDate = viewModel.NextMaintenanceDate.HasValue 
                    ? DateTime.SpecifyKind(viewModel.NextMaintenanceDate.Value, DateTimeKind.Utc) 
                    : null,
                ImageUrl = imageUrl,
                DocumentsUrl = viewModel.DocumentsUrl,
                Notes = viewModel.Notes,
                IsActive = viewModel.IsActive
            };

            var result = await _vehicleService.UpdateVehicleAsync(id, updateDto);

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.ErrorMessage;
                viewModel.VehicleTypes = GetVehicleTypeSelectList();
                return View(viewModel);
            }

            TempData["Success"] = "Vehicle updated successfully!";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating vehicle {VehicleId}", id);
            TempData["Error"] = "An error occurred while updating the vehicle.";
            viewModel.VehicleTypes = GetVehicleTypeSelectList();
            viewModel.Statuses = GetStatusSelectList();
            return View(viewModel);
        }
    }

    // GET: Vehicles/Delete/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _vehicleService.GetVehicleByIdAsync(id);
            if (!result.IsSuccess || result.Value == null)
            {
                TempData["Error"] = "Vehicle not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(MapToDetailsViewModel(result.Value));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading vehicle for delete {VehicleId}", id);
            TempData["Error"] = "An error occurred while loading the vehicle.";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Vehicles/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        try
        {
            var result = await _vehicleService.DeleteVehicleAsync(id);

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.ErrorMessage;
                return RedirectToAction(nameof(Delete), new { id });
            }

            TempData["Success"] = "Vehicle deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting vehicle {VehicleId}", id);
            TempData["Error"] = "An error occurred while deleting the vehicle.";
            return RedirectToAction(nameof(Index));
        }
    }


    // Helper methods
    private List<SelectListItem> GetVehicleTypeSelectList()
    {
        return Enum.GetValues(typeof(VehicleType))
            .Cast<VehicleType>()
            .Select(vt => new SelectListItem
            {
                Value = ((int)vt).ToString(),
                Text = vt.ToString()
            })
            .ToList();
    }

    private List<SelectListItem> GetStatusSelectList()
    {
        return Enum.GetValues(typeof(VehicleStatus))
            .Cast<VehicleStatus>()
            .Select(s => new SelectListItem
            {
                Value = ((int)s).ToString(),
                Text = s.ToString()
            })
            .ToList();
    }



    private VehicleListViewModel MapToListViewModel(Domain.DTOs.Vehicle.VehicleDto dto)
    {
        return new VehicleListViewModel
        {
            Id = dto.Id,
            PublicId = dto.PublicId,
            Brand = dto.Brand,
            Model = dto.Model,
            Year = dto.Year,
            LicensePlate = dto.LicensePlate,
            VehicleType = dto.VehicleType,
            DailyRate = dto.DailyRate,
            Status = dto.Status,
            IsActive = dto.IsActive,
            DisplayName = dto.DisplayName,
            IsAvailableForRent = dto.IsAvailableForRent,
            NeedsMaintenance = dto.NeedsMaintenance,
            CurrentHours = dto.CurrentHours,
            NextMaintenanceDate = dto.NextMaintenanceDate
        };
    }

    private VehicleDetailsViewModel MapToDetailsViewModel(Domain.DTOs.Vehicle.VehicleDto dto)
    {
        return new VehicleDetailsViewModel
        {
            Id = dto.Id,
            PublicId = dto.PublicId,
            Brand = dto.Brand,
            Model = dto.Model,
            Year = dto.Year,
            LicensePlate = dto.LicensePlate,
            VehicleType = dto.VehicleType,
            HourlyRate = dto.HourlyRate,
            DailyRate = dto.DailyRate,
            WeeklyRate = dto.WeeklyRate,
            MonthlyRate = dto.MonthlyRate,
            Status = dto.Status,
            IsActive = dto.IsActive,
            CurrentHours = dto.CurrentHours,
            CurrentMileage = dto.CurrentMileage,
            Specifications = dto.Specifications,
            SerialNumber = dto.SerialNumber,
            LastMaintenanceDate = dto.LastMaintenanceDate,
            NextMaintenanceDate = dto.NextMaintenanceDate,
            MaintenanceHoursInterval = dto.MaintenanceHoursInterval,
            ImageUrl = dto.ImageUrl,
            DocumentsUrl = dto.DocumentsUrl,
            Notes = dto.Notes,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            DisplayName = dto.DisplayName,
            IsAvailableForRent = dto.IsAvailableForRent,
            NeedsMaintenance = dto.NeedsMaintenance
        };
    }

    private VehicleFormViewModel MapToFormViewModel(Domain.DTOs.Vehicle.VehicleDto dto)
    {
        return new VehicleFormViewModel
        {
            Id = dto.Id,
            Brand = dto.Brand,
            Model = dto.Model,
            Year = dto.Year,
            LicensePlate = dto.LicensePlate,
            VehicleType = (int)Enum.Parse<VehicleType>(dto.VehicleType),
            Status = (int)Enum.Parse<VehicleStatus>(dto.Status),
            HourlyRate = dto.HourlyRate,
            DailyRate = dto.DailyRate,
            WeeklyRate = dto.WeeklyRate,
            MonthlyRate = dto.MonthlyRate,
            CurrentHours = dto.CurrentHours,
            CurrentMileage = dto.CurrentMileage,
            Specifications = dto.Specifications,
            SerialNumber = dto.SerialNumber,
            MaintenanceHoursInterval = dto.MaintenanceHoursInterval,
            LastMaintenanceDate = dto.LastMaintenanceDate,
            NextMaintenanceDate = dto.NextMaintenanceDate,
            ImageUrl = dto.ImageUrl,
            DocumentsUrl = dto.DocumentsUrl,
            Notes = dto.Notes,
            IsActive = dto.IsActive,
            VehicleTypes = GetVehicleTypeSelectList(),
            Statuses = GetStatusSelectList()
        };
    }

    // GET: Vehicles/ExportToExcel
    public async Task<IActionResult> ExportToExcel()
    {
        try
        {
            var excelData = await _exportService.ExportVehiclesToExcelAsync();
            return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Vehicles_{DateTime.Now:yyyyMMdd}.xlsx");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting vehicles to Excel");
            TempData["Error"] = "Error al exportar vehículos a Excel.";
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: Vehicles/ExportToPdf
    public async Task<IActionResult> ExportToPdf()
    {
        try
        {
            var pdfData = await _exportService.ExportVehiclesToPdfAsync();
            return File(pdfData, "application/pdf", $"Vehicles_{DateTime.Now:yyyyMMdd}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting vehicles to PDF");
            TempData["Error"] = "Error al exportar vehículos a PDF.";
            return RedirectToAction(nameof(Index));
        }
    }
}

