using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SteamInventoryBoosterV2.Migrations
{
    /// <inheritdoc />
    public partial class addOwnedItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OwnedItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AssetId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AcquiredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoldAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PurchasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ListedPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SalePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SteamItemClassId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OwnedItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OwnedItem_SteamItems_ClassId",
                        column: x => x.ClassId,
                        principalTable: "SteamItems",
                        principalColumn: "ClassId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OwnedItem_SteamItems_SteamItemClassId",
                        column: x => x.SteamItemClassId,
                        principalTable: "SteamItems",
                        principalColumn: "ClassId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_OwnedItem_ClassId",
                table: "OwnedItem",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_OwnedItem_SteamItemClassId",
                table: "OwnedItem",
                column: "SteamItemClassId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OwnedItem");
        }
    }
}
