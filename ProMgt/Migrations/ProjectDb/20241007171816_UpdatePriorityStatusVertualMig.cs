using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProMgt.Migrations.ProjectDb
{
    /// <inheritdoc />
    public partial class UpdatePriorityStatusVertualMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TaskStatuses_ColorId",
                table: "TaskStatuses",
                column: "ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_Priorities_ColorId",
                table: "Priorities",
                column: "ColorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Priorities_ProjectMgtColors_ColorId",
                table: "Priorities",
                column: "ColorId",
                principalTable: "ProjectMgtColors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskStatuses_ProjectMgtColors_ColorId",
                table: "TaskStatuses",
                column: "ColorId",
                principalTable: "ProjectMgtColors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Priorities_ProjectMgtColors_ColorId",
                table: "Priorities");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskStatuses_ProjectMgtColors_ColorId",
                table: "TaskStatuses");

            migrationBuilder.DropIndex(
                name: "IX_TaskStatuses_ColorId",
                table: "TaskStatuses");

            migrationBuilder.DropIndex(
                name: "IX_Priorities_ColorId",
                table: "Priorities");
        }
    }
}
