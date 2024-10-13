using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProMgt.Migrations.ProjectDb
{
    /// <inheritdoc />
    public partial class ProjectStatusMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_ProjectStatuses_ProjectStatusId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_ProjectStatusId",
                table: "Projects");

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "ProjectStatuses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_ProjectStatuses_ProjectStatusId1",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_ProjectStatusId1",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "ProjectStatuses");

            migrationBuilder.DropColumn(
                name: "ProjectStatusId1",
                table: "Projects");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectStatusId",
                table: "Projects",
                column: "ProjectStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_ProjectStatuses_ProjectStatusId",
                table: "Projects",
                column: "ProjectStatusId",
                principalTable: "ProjectStatuses",
                principalColumn: "Id");
        }
    }
}
