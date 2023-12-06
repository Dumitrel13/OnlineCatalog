using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineCatalog.Migrations
{
    /// <inheritdoc />
    public partial class ChangesToThePupilClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Absences_Pupils_PupilId",
                table: "Absences");

            migrationBuilder.DropForeignKey(
                name: "FK_FailedSubjects_Pupils_PupilId",
                table: "FailedSubjects");

            migrationBuilder.DropForeignKey(
                name: "FK_Grades_Pupils_PupilId",
                table: "Grades");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Pupils_PupilId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_ParentPupil_AspNetUsers_ParentsId",
                table: "ParentPupil");

            migrationBuilder.DropForeignKey(
                name: "FK_ParentPupil_Pupils_PupilsId",
                table: "ParentPupil");

            migrationBuilder.DropTable(
                name: "Pupils");

            migrationBuilder.AddColumn<int>(
                name: "ClassroomClassId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndingDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartingDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ClassroomClassId",
                table: "AspNetUsers",
                column: "ClassroomClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_Absences_AspNetUsers_PupilId",
                table: "Absences",
                column: "PupilId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Classrooms_ClassroomClassId",
                table: "AspNetUsers",
                column: "ClassroomClassId",
                principalTable: "Classrooms",
                principalColumn: "ClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_FailedSubjects_AspNetUsers_PupilId",
                table: "FailedSubjects",
                column: "PupilId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Grades_AspNetUsers_PupilId",
                table: "Grades",
                column: "PupilId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_PupilId",
                table: "Messages",
                column: "PupilId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_ParentPupil_AspNetUsers_ParentsId",
                table: "ParentPupil",
                column: "ParentsId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ParentPupil_AspNetUsers_PupilsId",
                table: "ParentPupil",
                column: "PupilsId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Absences_AspNetUsers_PupilId",
                table: "Absences");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Classrooms_ClassroomClassId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_FailedSubjects_AspNetUsers_PupilId",
                table: "FailedSubjects");

            migrationBuilder.DropForeignKey(
                name: "FK_Grades_AspNetUsers_PupilId",
                table: "Grades");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_PupilId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_ParentPupil_AspNetUsers_ParentsId",
                table: "ParentPupil");

            migrationBuilder.DropForeignKey(
                name: "FK_ParentPupil_AspNetUsers_PupilsId",
                table: "ParentPupil");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ClassroomClassId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ClassroomClassId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EndingDate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "StartingDate",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "Pupils",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassroomClassId = table.Column<int>(type: "int", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    EndingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StartingDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pupils", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pupils_Classrooms_ClassroomClassId",
                        column: x => x.ClassroomClassId,
                        principalTable: "Classrooms",
                        principalColumn: "ClassId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pupils_ClassroomClassId",
                table: "Pupils",
                column: "ClassroomClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_Absences_Pupils_PupilId",
                table: "Absences",
                column: "PupilId",
                principalTable: "Pupils",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FailedSubjects_Pupils_PupilId",
                table: "FailedSubjects",
                column: "PupilId",
                principalTable: "Pupils",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Grades_Pupils_PupilId",
                table: "Grades",
                column: "PupilId",
                principalTable: "Pupils",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Pupils_PupilId",
                table: "Messages",
                column: "PupilId",
                principalTable: "Pupils",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ParentPupil_AspNetUsers_ParentsId",
                table: "ParentPupil",
                column: "ParentsId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ParentPupil_Pupils_PupilsId",
                table: "ParentPupil",
                column: "PupilsId",
                principalTable: "Pupils",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
