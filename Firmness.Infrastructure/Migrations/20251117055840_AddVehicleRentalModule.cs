using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Firmness.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVehicleRentalModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Vehicle",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Brand = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Model = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    LicensePlate = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    VehicleType = table.Column<int>(type: "integer", nullable: false),
                    HourlyRate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DailyRate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    WeeklyRate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    MonthlyRate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CurrentHours = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    CurrentMileage = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Specifications = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    SerialNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LastMaintenanceDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NextMaintenanceDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MaintenanceHoursInterval = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DocumentsUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicle", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VehicleRental",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EstimatedReturnDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActualReturnDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    RentalRate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    RentalPeriodType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Deposit = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Tax = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Discount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PaidAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PendingAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PaymentMethod = table.Column<int>(type: "integer", nullable: false),
                    DepositReturned = table.Column<bool>(type: "boolean", nullable: false),
                    PickupLocation = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ReturnLocation = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ContractUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    InvoiceNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    InitialHours = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    FinalHours = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    InitialMileage = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    FinalMileage = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    InitialCondition = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    FinalCondition = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CancellationReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleRental", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleRental_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleRental_Vehicle_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_IsActive",
                table: "Vehicle",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_LicensePlate",
                table: "Vehicle",
                column: "LicensePlate",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_Status",
                table: "Vehicle",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_VehicleType",
                table: "Vehicle",
                column: "VehicleType");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleRental_CustomerId",
                table: "VehicleRental",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleRental_EstimatedReturnDate",
                table: "VehicleRental",
                column: "EstimatedReturnDate");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleRental_InvoiceNumber",
                table: "VehicleRental",
                column: "InvoiceNumber");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleRental_StartDate",
                table: "VehicleRental",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleRental_Status",
                table: "VehicleRental",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleRental_VehicleId",
                table: "VehicleRental",
                column: "VehicleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VehicleRental");

            migrationBuilder.DropTable(
                name: "Vehicle");
        }
    }
}
