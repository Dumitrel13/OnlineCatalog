using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineCatalog.Migrations
{
    /// <inheritdoc />
    public partial class ChangesToTheAbsenceClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "EndingHour",
                table: "Absences",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StartingHour",
                table: "Absences",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndingHour",
                table: "Absences");

            migrationBuilder.DropColumn(
                name: "StartingHour",
                table: "Absences");
        }
    }
}
