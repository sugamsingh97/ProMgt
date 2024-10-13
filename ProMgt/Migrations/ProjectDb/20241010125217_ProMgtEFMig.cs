using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProMgt.Migrations.ProjectDb
{
    /// <inheritdoc />
    public partial class ProMgtEFMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Priorities_ProjectMgtColors_ColorId",
                table: "Priorities");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Priorities_PriorityId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_TaskStatuses_TaskStatusId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskStatuses_ProjectMgtColors_ColorId",
                table: "TaskStatuses");

            migrationBuilder.AddForeignKey(
                name: "FK_Priorities_ProjectMgtColors_ColorId",
                table: "Priorities",
                column: "ColorId",
                principalTable: "ProjectMgtColors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Priorities_PriorityId",
                table: "Tasks",
                column: "PriorityId",
                principalTable: "Priorities",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_TaskStatuses_TaskStatusId",
                table: "Tasks",
                column: "TaskStatusId",
                principalTable: "TaskStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskStatuses_ProjectMgtColors_ColorId",
                table: "TaskStatuses",
                column: "ColorId",
                principalTable: "ProjectMgtColors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Priorities_ProjectMgtColors_ColorId",
                table: "Priorities");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Priorities_PriorityId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_TaskStatuses_TaskStatusId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskStatuses_ProjectMgtColors_ColorId",
                table: "TaskStatuses");

            migrationBuilder.AddForeignKey(
                name: "FK_Priorities_ProjectMgtColors_ColorId",
                table: "Priorities",
                column: "ColorId",
                principalTable: "ProjectMgtColors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Priorities_PriorityId",
                table: "Tasks",
                column: "PriorityId",
                principalTable: "Priorities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_TaskStatuses_TaskStatusId",
                table: "Tasks",
                column: "TaskStatusId",
                principalTable: "TaskStatuses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskStatuses_ProjectMgtColors_ColorId",
                table: "TaskStatuses",
                column: "ColorId",
                principalTable: "ProjectMgtColors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
