using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProMgt.Migrations.ProjectDb
{
    /// <inheritdoc />
    public partial class ProjectStatusNewUpdateMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColorId",
                table: "ProjectStatuses");

            migrationBuilder.AddColumn<string>(
                name: "HexCode",
                table: "ProjectStatuses",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HexCode",
                table: "ProjectStatuses");

            migrationBuilder.AddColumn<int>(
                name: "ColorId",
                table: "ProjectStatuses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
