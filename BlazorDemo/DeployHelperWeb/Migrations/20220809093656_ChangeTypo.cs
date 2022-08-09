using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeployHelperWeb.Migrations
{
    public partial class ChangeTypo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VersionNunber",
                table: "VersionItems",
                newName: "VersionNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VersionNumber",
                table: "VersionItems",
                newName: "VersionNunber");
        }
    }
}
