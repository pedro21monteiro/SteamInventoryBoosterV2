using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SteamInventoryBoosterV2.Migrations
{
    /// <inheritdoc />
    public partial class MakeClassIdPrimaryKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SteamItems",
                table: "SteamItems");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "SteamItems");

            migrationBuilder.AlterColumn<string>(
                name: "ClassId",
                table: "SteamItems",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SteamItems",
                table: "SteamItems",
                column: "ClassId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SteamItems",
                table: "SteamItems");

            migrationBuilder.AlterColumn<string>(
                name: "ClassId",
                table: "SteamItems",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "SteamItems",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SteamItems",
                table: "SteamItems",
                column: "Id");
        }
    }
}
