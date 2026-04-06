using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VgcCollege.Web.Migrations
{
    /// <inheritdoc />
    public partial class MakeStudentDobNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignmentResults_StudentProfiles_StudentProfileId",
                table: "AssignmentResults");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseEnrolments_StudentProfiles_StudentProfileId",
                table: "CourseEnrolments");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Branches_BranchId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamResults_StudentProfiles_StudentProfileId",
                table: "ExamResults");

            migrationBuilder.DropForeignKey(
                name: "FK_FacultyProfiles_AspNetUsers_IdentityUserId",
                table: "FacultyProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentProfiles_AspNetUsers_IdentityUserId",
                table: "StudentProfiles");

            migrationBuilder.DropIndex(
                name: "IX_StudentProfiles_IdentityUserId",
                table: "StudentProfiles");

            migrationBuilder.DropIndex(
                name: "IX_FacultyCourseAssignments_FacultyProfileId_CourseId",
                table: "FacultyCourseAssignments");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceRecords_CourseEnrolmentId",
                table: "AttendanceRecords");

            migrationBuilder.AlterColumn<string>(
                name: "IdentityUserId",
                table: "StudentProfiles",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<int>(
                name: "CourseId1",
                table: "CourseEnrolments",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "AttendanceRecords",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_FacultyCourseAssignments_FacultyProfileId",
                table: "FacultyCourseAssignments",
                column: "FacultyProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrolments_CourseId1",
                table: "CourseEnrolments",
                column: "CourseId1");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_CourseEnrolmentId_WeekNumber",
                table: "AttendanceRecords",
                columns: new[] { "CourseEnrolmentId", "WeekNumber" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentResults_StudentProfiles_StudentProfileId",
                table: "AssignmentResults",
                column: "StudentProfileId",
                principalTable: "StudentProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseEnrolments_Courses_CourseId1",
                table: "CourseEnrolments",
                column: "CourseId1",
                principalTable: "Courses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseEnrolments_StudentProfiles_StudentProfileId",
                table: "CourseEnrolments",
                column: "StudentProfileId",
                principalTable: "StudentProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Branches_BranchId",
                table: "Courses",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamResults_StudentProfiles_StudentProfileId",
                table: "ExamResults",
                column: "StudentProfileId",
                principalTable: "StudentProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FacultyProfiles_AspNetUsers_IdentityUserId",
                table: "FacultyProfiles",
                column: "IdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignmentResults_StudentProfiles_StudentProfileId",
                table: "AssignmentResults");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseEnrolments_Courses_CourseId1",
                table: "CourseEnrolments");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseEnrolments_StudentProfiles_StudentProfileId",
                table: "CourseEnrolments");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Branches_BranchId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamResults_StudentProfiles_StudentProfileId",
                table: "ExamResults");

            migrationBuilder.DropForeignKey(
                name: "FK_FacultyProfiles_AspNetUsers_IdentityUserId",
                table: "FacultyProfiles");

            migrationBuilder.DropIndex(
                name: "IX_FacultyCourseAssignments_FacultyProfileId",
                table: "FacultyCourseAssignments");

            migrationBuilder.DropIndex(
                name: "IX_CourseEnrolments_CourseId1",
                table: "CourseEnrolments");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceRecords_CourseEnrolmentId_WeekNumber",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "CourseId1",
                table: "CourseEnrolments");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "AttendanceRecords");

            migrationBuilder.AlterColumn<string>(
                name: "IdentityUserId",
                table: "StudentProfiles",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentProfiles_IdentityUserId",
                table: "StudentProfiles",
                column: "IdentityUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FacultyCourseAssignments_FacultyProfileId_CourseId",
                table: "FacultyCourseAssignments",
                columns: new[] { "FacultyProfileId", "CourseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_CourseEnrolmentId",
                table: "AttendanceRecords",
                column: "CourseEnrolmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentResults_StudentProfiles_StudentProfileId",
                table: "AssignmentResults",
                column: "StudentProfileId",
                principalTable: "StudentProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseEnrolments_StudentProfiles_StudentProfileId",
                table: "CourseEnrolments",
                column: "StudentProfileId",
                principalTable: "StudentProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Branches_BranchId",
                table: "Courses",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamResults_StudentProfiles_StudentProfileId",
                table: "ExamResults",
                column: "StudentProfileId",
                principalTable: "StudentProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FacultyProfiles_AspNetUsers_IdentityUserId",
                table: "FacultyProfiles",
                column: "IdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentProfiles_AspNetUsers_IdentityUserId",
                table: "StudentProfiles",
                column: "IdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
