using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineCatalog.Migrations
{
    /// <inheritdoc />
    public partial class ClassroomAndSubjectChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClassroomSubject",
                columns: table => new
                {
                    ClassroomsClassId = table.Column<int>(type: "int", nullable: false),
                    SubjectsSubjectId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassroomSubject", x => new { x.ClassroomsClassId, x.SubjectsSubjectId });
                    table.ForeignKey(
                        name: "FK_ClassroomSubject_Classrooms_ClassroomsClassId",
                        column: x => x.ClassroomsClassId,
                        principalTable: "Classrooms",
                        principalColumn: "ClassId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassroomSubject_Subjects_SubjectsSubjectId",
                        column: x => x.SubjectsSubjectId,
                        principalTable: "Subjects",
                        principalColumn: "SubjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassroomSubject_SubjectsSubjectId",
                table: "ClassroomSubject",
                column: "SubjectsSubjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassroomSubject");
        }
    }
}
