using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeployHelperWeb.Migrations
{
    public partial class AddPathField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "VersionItems",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Path",
                table: "VersionItems");
        }
    }
}
