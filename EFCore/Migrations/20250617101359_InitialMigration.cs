using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFCore.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HotStockDays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    OpeningComment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsSummaryExpanded = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotStockDays", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HotStockItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Market = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Price = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Change = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    ChangePercent = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    ReferencePrice = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    OpenPrice = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    MinPrice = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    MaxPrice = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Volume = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Turnover = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    HotStockDayId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotStockItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HotStockItems_HotStockDays_HotStockDayId",
                        column: x => x.HotStockDayId,
                        principalTable: "HotStockDays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HotStockItems_HotStockDayId",
                table: "HotStockItems",
                column: "HotStockDayId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HotStockItems");

            migrationBuilder.DropTable(
                name: "HotStockDays");
        }
    }
}
