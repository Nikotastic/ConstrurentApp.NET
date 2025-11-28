﻿using Firmness.Application.Interfaces;
using Firmness.Domain.DTOs.Vehicle;
using Firmness.Domain.Enums;
using Firmness.Web.ViewModels.VehicleRental;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Firmness.Web.Controllers;

[Authorize]
public class VehicleRentalsController : Controller
{
    private readonly IVehicleRentalService _rentalService;
    private readonly IVehicleService _vehicleService;
    private readonly ICustomerService _customerService;
    private readonly IExportService _exportService;
    private readonly ILogger<VehicleRentalsController> _logger;

    public VehicleRentalsController(
        IVehicleRentalService rentalService,
        IVehicleService vehicleService,
        ICustomerService customerService,
        IExportService exportService,
        ILogger<VehicleRentalsController> logger)
    {
        _rentalService = rentalService;
        _vehicleService = vehicleService;
        _customerService = customerService;
        _exportService = exportService;
        _logger = logger;
    }

    // GET: VehicleRentals
    public async Task<IActionResult> Index(int page = 1, int pageSize = 20, string? status = null, string? search = null)
    {
        // Prepare status dropdown - always set this
        ViewBag.Statuses = Enum.GetNames(typeof(RentalStatus))
            .Select(name => new SelectListItem { Value = name, Text = name })
            .ToList();

        try
        {
            var result = await _rentalService.GetAllRentalsWithDetailsAsync();
            
            if (!result.IsSuccess || result.Value == null)
            {
                TempData["Error"] = "Could not load rentals.";
                return View(new PaginatedVehicleRentalListViewModel
                {
                    Rentals = new List<VehicleRentalListViewModel>(),
                    CurrentPage = 1,
                    PageSize = pageSize,
                    TotalItems = 0,
                    TotalPages = 0
                });
            }

            var rentals = result.Value.Select(MapToListViewModel).ToList();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(status))
            {
                rentals = rentals.Where(r => r.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                rentals = rentals.Where(r =>
                    r.CustomerName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    r.VehicleDisplayName.Contains(search, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            // Apply pagination
            var totalItems = rentals.Count;
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            
            var paginatedRentals = rentals
                .OrderByDescending(r => r.StartDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewModel = new PaginatedVehicleRentalListViewModel
            {
                Rentals = paginatedRentals,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Status = status,
                Search = search
            };


            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading rentals");
            TempData["Error"] = "An error occurred while loading rentals.";
            return View(new PaginatedVehicleRentalListViewModel
            {
                Rentals = new List<VehicleRentalListViewModel>(),
                CurrentPage = 1,
                PageSize = pageSize,
                TotalItems = 0,
                TotalPages = 0
            });
        }
    }

    // GET: VehicleRentals/Details/5
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var result = await _rentalService.GetRentalByIdWithDetailsAsync(id);
            if (!result.IsSuccess || result.Value == null)
            {
                TempData["Error"] = "Rental not found.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = MapToDetailsViewModel(result.Value);
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading rental details for {RentalId}", id);
            TempData["Error"] = "An error occurred while loading rental details.";
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: VehicleRentals/Create
    public async Task<IActionResult> Create()
    {
        try
        {
            var viewModel = new VehicleRentalFormViewModel();
            await PopulateDropdownsAsync(viewModel);
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading create rental form");
            TempData["Error"] = "Error loading form.";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: VehicleRentals/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VehicleRentalFormViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(model);
                return View(model);
            }

            // Create DTO from ViewModel
            var createDto = new CreateVehicleRentalDto
            {
                CustomerId = model.CustomerId,
                VehicleId = model.VehicleId,
                StartDate = model.StartDate,
                EstimatedReturnDate = model.EstimatedReturnDate,
                RentalRate = model.RentalRate,
                RentalPeriodType = model.RentalPeriodType,
                Deposit = model.Deposit,
                PickupLocation = model.PickupLocation ?? string.Empty,
                ReturnLocation = model.ReturnLocation ?? string.Empty,
                InitialHours = model.InitialHours,
                InitialMileage = model.InitialMileage,
                InitialCondition = model.InitialCondition ?? string.Empty,
                Notes = model.Notes ?? string.Empty
            };

            var result = await _rentalService.CreateRentalAsync(createDto);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.ErrorMessage ?? "Error creating rental.");
                await PopulateDropdownsAsync(model);
                return View(model);
            }

            TempData["Success"] = "Rental created successfully!";
            return RedirectToAction(nameof(Details), new { id = result.Value });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating rental");
            ModelState.AddModelError("", "An error occurred while creating the rental.");
            await PopulateDropdownsAsync(model);
            return View(model);
        }
    }

    // Helper method to populate dropdowns
    private async Task PopulateDropdownsAsync(VehicleRentalFormViewModel model)
    {
        // Get available vehicles
        var vehiclesResult = await _vehicleService.GetAvailableVehiclesAsync();
        model.Vehicles = vehiclesResult.IsSuccess && vehiclesResult.Value != null
            ? vehiclesResult.Value.Select(v => new SelectListItem
            {
                Value = v.Id.ToString(),
                Text = v.DisplayName
            }).ToList()
            : new List<SelectListItem>();

        // Get active customers
        var allCustomers = await _customerService.GetAllAsync();
        model.Customers = allCustomers
            .Where(c => c.IsActive)
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = $"{c.FullName} - {c.Document}"
            }).ToList();

        // Rental period types
        model.RentalPeriodTypes = Enum.GetValues(typeof(RentalPeriodType))
            .Cast<RentalPeriodType>()
            .Select(rpt => new SelectListItem
            {
                Value = rpt.ToString(),
                Text = rpt.ToString()
            }).ToList();

        // Payment methods
        model.PaymentMethods = Enum.GetValues(typeof(PaymentMethod))
            .Cast<PaymentMethod>()
            .Select(pm => new SelectListItem
            {
                Value = ((int)pm).ToString(),
                Text = pm.ToString()
            }).ToList();
    }

    // GET: VehicleRentals/ExportToExcel
    public async Task<IActionResult> ExportToExcel()
    {
        try
        {
            var excelData = await _exportService.ExportVehicleRentalsToExcelAsync();
            return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"VehicleRentals_{DateTime.Now:yyyyMMdd}.xlsx");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting vehicle rentals to Excel");
            TempData["Error"] = "Error exporting rents to Excel.";
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: VehicleRentals/ExportToPdf
    public async Task<IActionResult> ExportToPdf()
    {
        try
        {
            var pdfData = await _exportService.ExportVehicleRentalsToPdfAsync();
            return File(pdfData, "application/pdf", $"VehicleRentals_{DateTime.Now:yyyyMMdd}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting vehicle rentals to PDF");
            TempData["Error"] = "Error exporting rents to PDF.";
            return RedirectToAction(nameof(Index));
        }
    }

    // Helper methods
    private VehicleRentalListViewModel MapToListViewModel(VehicleRentalDto dto)
    {
        return new VehicleRentalListViewModel
        {
            Id = dto.Id,
            CustomerName = dto.CustomerName,
            VehicleDisplayName = dto.VehicleDisplayName,
            StartDate = dto.StartDate,
            EstimatedReturnDate = dto.EstimatedReturnDate,
            ActualReturnDate = dto.ActualReturnDate,
            Status = dto.Status,
            TotalAmount = dto.TotalAmount,
            PendingAmount = dto.PendingAmount,
            IsOverdue = dto.IsOverdue,
            PickupLocation = dto.PickupLocation,
            DurationInDays = dto.DurationInDays
        };
    }

    private VehicleRentalDetailsViewModel MapToDetailsViewModel(VehicleRentalDto dto)
    {
        return new VehicleRentalDetailsViewModel
        {
            Id = dto.Id,
            CustomerId = dto.CustomerId,
            CustomerName = dto.CustomerName,
            VehicleId = dto.VehicleId,
            VehicleDisplayName = dto.VehicleDisplayName,
            VehicleBrand = dto.VehicleBrand,
            VehicleModel = dto.VehicleModel,
            VehicleLicensePlate = dto.VehicleLicensePlate,
            StartDate = dto.StartDate,
            EstimatedReturnDate = dto.EstimatedReturnDate,
            ActualReturnDate = dto.ActualReturnDate,
            Status = dto.Status,
            RentalRate = dto.RentalRate,
            RentalPeriodType = dto.RentalPeriodType,
            Deposit = dto.Deposit,
            Subtotal = dto.Subtotal,
            Tax = dto.Tax,
            Discount = dto.Discount,
            TotalAmount = dto.TotalAmount,
            PaidAmount = dto.PaidAmount,
            PendingAmount = dto.PendingAmount,
            PaymentMethod = dto.PaymentMethod,
            DepositReturned = dto.DepositReturned,
            PickupLocation = dto.PickupLocation,
            ReturnLocation = dto.ReturnLocation,
            ContractUrl = dto.ContractUrl,
            InvoiceNumber = dto.InvoiceNumber,
            InitialHours = dto.InitialHours,
            FinalHours = dto.FinalHours,
            InitialMileage = dto.InitialMileage,
            FinalMileage = dto.FinalMileage,
            InitialCondition = dto.InitialCondition,
            FinalCondition = dto.FinalCondition,
            Notes = dto.Notes,
            CancellationReason = dto.CancellationReason,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            DurationInDays = dto.DurationInDays,
            IsOverdue = dto.IsOverdue
        };
    }
}

