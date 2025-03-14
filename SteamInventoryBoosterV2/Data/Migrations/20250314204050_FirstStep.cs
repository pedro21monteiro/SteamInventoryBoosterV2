using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SteamInventoryBoosterV2.Migrations
{
    /// <inheritdoc />
    public partial class FirstStep : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SteamItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Link = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    HighestPurchaseOrder = table.Column<double>(type: "float", nullable: false),
                    LastVerification = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BuyPrice = table.Column<double>(type: "float", nullable: true),
                    SellPrice = table.Column<double>(type: "float", nullable: true),
                    IconPath = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SteamItems", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SteamItems_Link",
                table: "SteamItems",
                column: "Link",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SteamItems_Name",
                table: "SteamItems",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SteamItems");
        }
    }
}
