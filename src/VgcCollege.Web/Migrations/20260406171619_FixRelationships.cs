using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VgcCollege.Web.Migrations
{
    /// <inheritdoc />
    public partial class FixRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "FacultyProfiles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "FacultyProfiles",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
