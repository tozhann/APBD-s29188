using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserPanelApp.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppUsers",
                columns: table => new
                {
                    Id           = table.Column<int>(nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    Email        = table.Column<string>(maxLength: 160, nullable: false),
                    PasswordHash = table.Column<string>(nullable: false),
                    Role         = table.Column<string>(maxLength: 30, nullable: false),
                    CreatedAt    = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserNotes",
                columns: table => new
                {
                    Id        = table.Column<int>(nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    AppUserId = table.Column<int>(nullable: false),
                    Title     = table.Column<string>(maxLength: 160, nullable: false),
                    Content   = table.Column<string>(maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserNotes_AppUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_Email",
                table: "AppUsers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserNotes_AppUserId",
                table: "UserNotes",
                column: "AppUserId");

            // Seed admin user (password: Admin@1234)
            migrationBuilder.InsertData(
                table: "AppUsers",
                columns: new[] { "Id", "Email", "PasswordHash", "Role", "CreatedAt" },
                values: new object[]
                {
                    1,
                    "admin@example.com",
                    "$2a$11$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi", // Admin@1234
                    "Admin",
                    new DateTime(2026, 1, 1, 0, 0, 0)
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "UserNotes");
            migrationBuilder.DropTable(name: "AppUsers");
        }
    }
}
