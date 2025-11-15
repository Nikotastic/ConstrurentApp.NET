using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Firmness.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryAndEnumsUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // PRIMERO: Limpiar SKUs duplicados antes de crear el índice único
            migrationBuilder.Sql(@"
                DELETE FROM ""Product""
                WHERE ""Id"" IN (
                    SELECT ""Id""
                    FROM (
                        SELECT ""Id"", ""SKU"",
                               ROW_NUMBER() OVER (PARTITION BY ""SKU"" ORDER BY ""Id"") as rn
                        FROM ""Product""
                    ) t
                    WHERE t.rn > 1
                );
            ");

            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "Sale",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNumber",
                table: "Sale",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Sale",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethod",
                table: "Sale",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Sale",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Subtotal",
                table: "Sale",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Tax",
                table: "Sale",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Barcode",
                table: "Product",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "Product",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Cost",
                table: "Product",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Product",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "MinStock",
                table: "Product",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sale_InvoiceNumber",
                table: "Sale",
                column: "InvoiceNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Sale_Status",
                table: "Sale",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Product_Barcode",
                table: "Product",
                column: "Barcode");

            migrationBuilder.CreateIndex(
                name: "IX_Product_CategoryId",
                table: "Product",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_SKU",
                table: "Product",
                column: "SKU",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Category_Name",
                table: "Category",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Category_CategoryId",
                table: "Product",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_Category_CategoryId",
                table: "Product");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropIndex(
                name: "IX_Sale_InvoiceNumber",
                table: "Sale");

            migrationBuilder.DropIndex(
                name: "IX_Sale_Status",
                table: "Sale");

            migrationBuilder.DropIndex(
                name: "IX_Product_Barcode",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_CategoryId",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_SKU",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "Sale");

            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                table: "Sale");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Sale");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Sale");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Sale");

            migrationBuilder.DropColumn(
                name: "Subtotal",
                table: "Sale");

            migrationBuilder.DropColumn(
                name: "Tax",
                table: "Sale");

            migrationBuilder.DropColumn(
                name: "Barcode",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Cost",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "MinStock",
                table: "Product");
        }
    }
}
