using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Firmness.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncModel20251106 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Product");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "User",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "User",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "SaleItem",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "SaleItem",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Sale",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Customer",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Customer",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Document",
                table: "Customer",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Customer",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastPasswordChangeDate",
                table: "Customer",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Customer",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhotoFile",
                table: "Customer",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "Customer",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Customer",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "User");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "User");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "SaleItem");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "SaleItem");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Sale");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "Document",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "LastPasswordChangeDate",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "PhotoFile",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Customer");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Product",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Product",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
