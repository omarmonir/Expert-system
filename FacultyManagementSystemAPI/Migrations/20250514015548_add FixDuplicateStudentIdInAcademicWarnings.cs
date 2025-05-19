using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacultyManagementSystemAPI.Migrations
{
    /// <inheritdoc />
    public partial class addFixDuplicateStudentIdInAcademicWarnings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcademicWarnings_Students_StudentId1",
                table: "AcademicWarnings");

            migrationBuilder.DropIndex(
                name: "IX_AcademicWarnings_StudentId1",
                table: "AcademicWarnings");

            migrationBuilder.DropColumn(
                name: "StudentId1",
                table: "AcademicWarnings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StudentId1",
                table: "AcademicWarnings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AcademicWarnings_StudentId1",
                table: "AcademicWarnings",
                column: "StudentId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AcademicWarnings_Students_StudentId1",
                table: "AcademicWarnings",
                column: "StudentId1",
                principalTable: "Students",
                principalColumn: "Id");
        }
    }
}
