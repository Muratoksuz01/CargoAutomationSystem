using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CargoAutomationSystem.Migrations
{
    /// <inheritdoc />
    public partial class SmallFix2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id",
                table: "UserCargos",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "BranchCargos",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "UserCargos",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "BranchCargos",
                newName: "id");
        }
    }
}
