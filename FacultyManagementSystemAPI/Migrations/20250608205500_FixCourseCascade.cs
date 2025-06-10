using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacultyManagementSystemAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixCourseCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CoursePrerequisites_Courses_CourseId",
                table: "CoursePrerequisites");

            migrationBuilder.DropForeignKey(
                name: "FK_CoursePrerequisites_Courses_PrerequisiteCourseId",
                table: "CoursePrerequisites");

            migrationBuilder.AddForeignKey(
                name: "FK_CoursePrerequisites_Courses_CourseId",
                table: "CoursePrerequisites",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CoursePrerequisites_Courses_PrerequisiteCourseId",
                table: "CoursePrerequisites",
                column: "PrerequisiteCourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CoursePrerequisites_Courses_CourseId",
                table: "CoursePrerequisites");

            migrationBuilder.DropForeignKey(
                name: "FK_CoursePrerequisites_Courses_PrerequisiteCourseId",
                table: "CoursePrerequisites");

            migrationBuilder.AddForeignKey(
                name: "FK_CoursePrerequisites_Courses_CourseId",
                table: "CoursePrerequisites",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CoursePrerequisites_Courses_PrerequisiteCourseId",
                table: "CoursePrerequisites",
                column: "PrerequisiteCourseId",
                principalTable: "Courses",
                principalColumn: "Id");
        }
    }
}
