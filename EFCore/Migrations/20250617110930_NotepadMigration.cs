using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFCore.Migrations
{
    /// <inheritdoc />
    public partial class NotepadMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompanyItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompanyNotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NotepadCompanyItemId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyNotes_CompanyItems_NotepadCompanyItemId",
                        column: x => x.NotepadCompanyItemId,
                        principalTable: "CompanyItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanySummaries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NotepadCompanyItemId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanySummaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanySummaries_CompanyItems_NotepadCompanyItemId",
                        column: x => x.NotepadCompanyItemId,
                        principalTable: "CompanyItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyNotes_NotepadCompanyItemId",
                table: "CompanyNotes",
                column: "NotepadCompanyItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanySummaries_NotepadCompanyItemId",
                table: "CompanySummaries",
                column: "NotepadCompanyItemId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyNotes");

            migrationBuilder.DropTable(
                name: "CompanySummaries");

            migrationBuilder.DropTable(
                name: "CompanyItems");
        }
    }
}
