using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineCatalog.Migrations
{
    /// <inheritdoc />
    public partial class AddedNewPropertiesToTeacherAssignmentClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApplicationRoleId",
                table: "TeacherAssignments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndingDate",
                table: "TeacherAssignments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartingDate",
                table: "TeacherAssignments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_TeacherAssignments_ApplicationRoleId",
                table: "TeacherAssignments",
                column: "ApplicationRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherAssignments_AspNetRoles_ApplicationRoleId",
                table: "TeacherAssignments",
                column: "ApplicationRoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeacherAssignments_AspNetRoles_ApplicationRoleId",
                table: "TeacherAssignments");

            migrationBuilder.DropIndex(
                name: "IX_TeacherAssignments_ApplicationRoleId",
                table: "TeacherAssignments");

            migrationBuilder.DropColumn(
                name: "ApplicationRoleId",
                table: "TeacherAssignments");

            migrationBuilder.DropColumn(
                name: "EndingDate",
                table: "TeacherAssignments");

            migrationBuilder.DropColumn(
                name: "StartingDate",
                table: "TeacherAssignments");
        }
    }
}
