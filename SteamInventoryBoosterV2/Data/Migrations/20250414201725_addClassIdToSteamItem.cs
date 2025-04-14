using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SteamInventoryBoosterV2.Migrations
{
    /// <inheritdoc />
    public partial class addClassIdToSteamItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClassId",
                table: "SteamItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "SteamItems");
        }
    }
}
