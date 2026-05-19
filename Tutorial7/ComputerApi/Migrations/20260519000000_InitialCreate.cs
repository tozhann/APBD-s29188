using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814

namespace ComputerApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ComponentManufacturers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Abbreviation = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    FoundationDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentManufacturers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ComponentTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Abbreviation = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PCs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Weight = table.Column<float>(type: "real", nullable: false),
                    Warranty = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PCs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Components",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(10)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ComponentManufacturersId = table.Column<int>(type: "int", nullable: false),
                    ComponentTypesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Components", x => x.Code);
                    table.ForeignKey(
                        name: "FK_Components_ComponentManufacturers_ComponentManufacturersId",
                        column: x => x.ComponentManufacturersId,
                        principalTable: "ComponentManufacturers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Components_ComponentTypes_ComponentTypesId",
                        column: x => x.ComponentTypesId,
                        principalTable: "ComponentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PCComponents",
                columns: table => new
                {
                    PCId = table.Column<int>(type: "int", nullable: false),
                    ComponentCode = table.Column<string>(type: "char(10)", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PCComponents", x => new { x.PCId, x.ComponentCode });
                    table.ForeignKey(
                        name: "FK_PCComponents_Components_ComponentCode",
                        column: x => x.ComponentCode,
                        principalTable: "Components",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PCComponents_PCs_PCId",
                        column: x => x.PCId,
                        principalTable: "PCs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // ── Seed Data ──────────────────────────────────────────────

            migrationBuilder.InsertData(
                table: "ComponentTypes",
                columns: new[] { "Id", "Abbreviation", "Name" },
                values: new object[,]
                {
                    { 1, "CPU", "Central Processing Unit" },
                    { 2, "GPU", "Graphics Processing Unit" },
                    { 3, "RAM", "Random Access Memory" }
                });

            migrationBuilder.InsertData(
                table: "ComponentManufacturers",
                columns: new[] { "Id", "Abbreviation", "FullName", "FoundationDate" },
                values: new object[,]
                {
                    { 1, "Intel", "Intel Corporation", new DateOnly(1968, 7, 18) },
                    { 2, "AMD", "Advanced Micro Devices", new DateOnly(1969, 5, 1) },
                    { 3, "NVIDIA", "NVIDIA Corporation", new DateOnly(1993, 4, 5) }
                });

            migrationBuilder.InsertData(
                table: "Components",
                columns: new[] { "Code", "Name", "Description", "ComponentManufacturersId", "ComponentTypesId" },
                values: new object[,]
                {
                    { "CPU001    ", "Intel Core i9-13900K", "High-end desktop processor", 1, 1 },
                    { "GPU001    ", "NVIDIA RTX 4090", "Flagship gaming GPU", 3, 2 },
                    { "RAM001    ", "Corsair DDR5 32GB", "High-speed DDR5 memory", 2, 3 }
                });

            migrationBuilder.InsertData(
                table: "PCs",
                columns: new[] { "Id", "Name", "Weight", "Warranty", "CreatedAt", "Stock" },
                values: new object[,]
                {
                    { 1, "Gaming Beast X", 12.5f, 36, new DateTime(2026, 5, 8, 9, 0, 0), 5 },
                    { 2, "Office Mini Pro", 4.2f, 24, new DateTime(2026, 4, 15, 13, 30, 0), 12 },
                    { 3, "Workstation Ultra", 18.0f, 48, new DateTime(2026, 3, 1, 8, 0, 0), 3 }
                });

            migrationBuilder.InsertData(
                table: "PCComponents",
                columns: new[] { "PCId", "ComponentCode", "Amount" },
                values: new object[,]
                {
                    { 1, "CPU001    ", 1 },
                    { 1, "GPU001    ", 2 },
                    { 2, "RAM001    ", 2 },
                    { 3, "CPU001    ", 2 },
                    { 3, "GPU001    ", 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Components_ComponentManufacturersId",
                table: "Components",
                column: "ComponentManufacturersId");

            migrationBuilder.CreateIndex(
                name: "IX_Components_ComponentTypesId",
                table: "Components",
                column: "ComponentTypesId");

            migrationBuilder.CreateIndex(
                name: "IX_PCComponents_ComponentCode",
                table: "PCComponents",
                column: "ComponentCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "PCComponents");
            migrationBuilder.DropTable(name: "Components");
            migrationBuilder.DropTable(name: "PCs");
            migrationBuilder.DropTable(name: "ComponentTypes");
            migrationBuilder.DropTable(name: "ComponentManufacturers");
        }
    }
}
