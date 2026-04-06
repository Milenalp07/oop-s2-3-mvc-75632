using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VgcCollege.Web.Migrations
{
    /// <inheritdoc />
    public partial class FixCourseEnrolmentRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseEnrolments_Courses_CourseId1",
                table: "CourseEnrolments");

            migrationBuilder.DropIndex(
                name: "IX_CourseEnrolments_CourseId1",
                table: "CourseEnrolments");

            migrationBuilder.DropColumn(
                name: "CourseId1",
                table: "CourseEnrolments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CourseId1",
                table: "CourseEnrolments",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrolments_CourseId1",
                table: "CourseEnrolments",
                column: "CourseId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseEnrolments_Courses_CourseId1",
                table: "CourseEnrolments",
                column: "CourseId1",
                principalTable: "Courses",
                principalColumn: "Id");
        }
    }
}
