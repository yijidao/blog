using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeployHelperWeb.Migrations
{
    public partial class AddNameAndCreator : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Creator",
                table: "VersionItems",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "VersionItems",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Creator",
                table: "VersionItems");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "VersionItems");
        }
    }
}
