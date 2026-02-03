using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ECommAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: false),
                    Stock = table.Column<int>(type: "INTEGER", nullable: false),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    SessionId = table.Column<string>(type: "TEXT", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Electronic devices and gadgets", "Electronics" },
                    { 2, "Fashion and apparel", "Clothing" },
                    { 3, "Books and literature", "Books" },
                    { 4, "Home improvement and gardening", "Home & Garden" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Description", "ImageUrl", "Name", "Price", "Stock" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 2, 3, 12, 12, 54, 478, DateTimeKind.Utc).AddTicks(4522), "15.6 inch, Intel i5, 8GB RAM, 256GB SSD", "https://via.placeholder.com/300x300?text=Laptop", "Laptop HP Pavilion", 699.99m, 15 },
                    { 2, 1, new DateTime(2026, 2, 3, 12, 12, 54, 478, DateTimeKind.Utc).AddTicks(5017), "Ergonomic wireless mouse with USB receiver", "https://via.placeholder.com/300x300?text=Mouse", "Wireless Mouse", 25.99m, 50 },
                    { 3, 1, new DateTime(2026, 2, 3, 12, 12, 54, 478, DateTimeKind.Utc).AddTicks(5021), "6.5 inch display, 128GB storage, 5G enabled", "https://via.placeholder.com/300x300?text=Phone", "Smartphone Samsung Galaxy", 599.99m, 30 },
                    { 4, 2, new DateTime(2026, 2, 3, 12, 12, 54, 478, DateTimeKind.Utc).AddTicks(5023), "100% cotton, available in multiple colors", "https://via.placeholder.com/300x300?text=TShirt", "Men's T-Shirt", 19.99m, 100 },
                    { 5, 2, new DateTime(2026, 2, 3, 12, 12, 54, 478, DateTimeKind.Utc).AddTicks(5027), "Slim fit denim jeans", "https://via.placeholder.com/300x300?text=Jeans", "Women's Jeans", 49.99m, 60 },
                    { 6, 3, new DateTime(2026, 2, 3, 12, 12, 54, 478, DateTimeKind.Utc).AddTicks(5030), "A Handbook of Agile Software Craftsmanship", "https://via.placeholder.com/300x300?text=Book", "Clean Code", 32.99m, 25 },
                    { 7, 3, new DateTime(2026, 2, 3, 12, 12, 54, 478, DateTimeKind.Utc).AddTicks(5033), "Your Journey to Mastery", "https://via.placeholder.com/300x300?text=Book", "The Pragmatic Programmer", 39.99m, 20 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ProductId",
                table: "CartItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
