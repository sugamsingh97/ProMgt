using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProMgt.Migrations.ProjectDb
{
    /// <inheritdoc />
    public partial class UpdatePriorityStatusMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Priorities_PriorityId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_TaskStatuses_TaskStatusId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_PriorityId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_TaskStatusId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "PriorityId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "TaskStatusId",
                table: "Tasks");

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "TaskStatuses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "projectId",
                table: "Priorities",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "TaskStatuses");

            migrationBuilder.DropColumn(
                name: "projectId",
                table: "Priorities");

            migrationBuilder.AddColumn<int>(
                name: "PriorityId",
                table: "Tasks",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TaskStatusId",
                table: "Tasks",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_PriorityId",
                table: "Tasks",
                column: "PriorityId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TaskStatusId",
                table: "Tasks",
                column: "TaskStatusId",
                unique: true);

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
        }
    }
}
