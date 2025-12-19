using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Firmness.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeHoursAndMileageToNoDecimals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "InitialMileage",
                table: "VehicleRental",
                type: "numeric(18,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "InitialHours",
                table: "VehicleRental",
                type: "numeric(18,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "FinalMileage",
                table: "VehicleRental",
                type: "numeric(18,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "FinalHours",
                table: "VehicleRental",
                type: "numeric(18,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MaintenanceHoursInterval",
                table: "Vehicle",
                type: "numeric(18,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentMileage",
                table: "Vehicle",
                type: "numeric(18,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentHours",
                table: "Vehicle",
                type: "numeric(18,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "InitialMileage",
                table: "VehicleRental",
                type: "numeric(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,0)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "InitialHours",
                table: "VehicleRental",
                type: "numeric(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,0)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "FinalMileage",
                table: "VehicleRental",
                type: "numeric(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,0)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "FinalHours",
                table: "VehicleRental",
                type: "numeric(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,0)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MaintenanceHoursInterval",
                table: "Vehicle",
                type: "numeric(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,0)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentMileage",
                table: "Vehicle",
                type: "numeric(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,0)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentHours",
                table: "Vehicle",
                type: "numeric(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,0)",
                oldNullable: true);
        }
    }
}
