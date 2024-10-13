using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProMgt.Migrations.ProjectDb
{
    /// <inheritdoc />
    public partial class NewProjMgtDbMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Priorities_PriorityId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Projects_ProjectId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Sections_SectionId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_TaskStatuses_TaskStatusId",
                table: "Tasks");

            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "Sections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tasks",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_SectionId",
                table: "Tasks");

            migrationBuilder.RenameTable(
                name: "Tasks",
                newName: "ProjectTasks");

            migrationBuilder.RenameColumn(
                name: "projectId",
                table: "Priorities",
                newName: "ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_TaskStatusId",
                table: "ProjectTasks",
                newName: "IX_ProjectTasks_TaskStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_ProjectId",
                table: "ProjectTasks",
                newName: "IX_ProjectTasks_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_PriorityId",
                table: "ProjectTasks",
                newName: "IX_ProjectTasks_PriorityId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectTasks",
                table: "ProjectTasks",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TaskStatuses_ProjectId",
                table: "TaskStatuses",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectStatusId",
                table: "Projects",
                column: "ProjectStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Priorities_ProjectId",
                table: "Priorities",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Priorities_Projects_ProjectId",
                table: "Priorities",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_ProjectStatuses_ProjectStatusId",
                table: "Projects",
                column: "ProjectStatusId",
                principalTable: "ProjectStatuses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTasks_Priorities_PriorityId",
                table: "ProjectTasks",
                column: "PriorityId",
                principalTable: "Priorities",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTasks_Projects_ProjectId",
                table: "ProjectTasks",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTasks_TaskStatuses_TaskStatusId",
                table: "ProjectTasks",
                column: "TaskStatusId",
                principalTable: "TaskStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskStatuses_Projects_ProjectId",
                table: "TaskStatuses",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Priorities_Projects_ProjectId",
                table: "Priorities");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_ProjectStatuses_ProjectStatusId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTasks_Priorities_PriorityId",
                table: "ProjectTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTasks_Projects_ProjectId",
                table: "ProjectTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTasks_TaskStatuses_TaskStatusId",
                table: "ProjectTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskStatuses_Projects_ProjectId",
                table: "TaskStatuses");

            migrationBuilder.DropIndex(
                name: "IX_TaskStatuses_ProjectId",
                table: "TaskStatuses");

            migrationBuilder.DropIndex(
                name: "IX_Projects_ProjectStatusId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Priorities_ProjectId",
                table: "Priorities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectTasks",
                table: "ProjectTasks");

            migrationBuilder.RenameTable(
                name: "ProjectTasks",
                newName: "Tasks");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "Priorities",
                newName: "projectId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectTasks_TaskStatusId",
                table: "Tasks",
                newName: "IX_Tasks_TaskStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectTasks_ProjectId",
                table: "Tasks",
                newName: "IX_Tasks_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectTasks_PriorityId",
                table: "Tasks",
                newName: "IX_Tasks_PriorityId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tasks",
                table: "Tasks",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    IsUser = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    ProjectOwner = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProjectId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sections_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_SectionId",
                table: "Tasks",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Sections_ProjectId",
                table: "Sections",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Priorities_PriorityId",
                table: "Tasks",
                column: "PriorityId",
                principalTable: "Priorities",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Projects_ProjectId",
                table: "Tasks",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Sections_SectionId",
                table: "Tasks",
                column: "SectionId",
                principalTable: "Sections",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_TaskStatuses_TaskStatusId",
                table: "Tasks",
                column: "TaskStatusId",
                principalTable: "TaskStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
