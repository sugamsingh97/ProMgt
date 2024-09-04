using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProMgt.Migrations.ProjectDb
{
    /// <inheritdoc />
    public partial class DeadLineAddedMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeadLine",
                table: "Projects",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeadLine",
                table: "Projects");
        }
    }
}
