using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineCatalog.Migrations
{
    /// <inheritdoc />
    public partial class AddedANewPropertyToTheMessageClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSentByParent",
                table: "Messages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSentByParent",
                table: "Messages");
        }
    }
}
