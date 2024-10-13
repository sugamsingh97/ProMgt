using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProMgt.Migrations.ProjectDb
{
    /// <inheritdoc />
    public partial class UpdatedProStatusMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_ProjectStatuses_ProjectStatusId1",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_ProjectStatusId1",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ProjectStatusId1",
                table: "Projects");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectStatusId1",
                table: "Projects",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectStatusId1",
                table: "Projects",
                column: "ProjectStatusId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_ProjectStatuses_ProjectStatusId1",
                table: "Projects",
                column: "ProjectStatusId1",
                principalTable: "ProjectStatuses",
                principalColumn: "Id");
        }
    }
}
