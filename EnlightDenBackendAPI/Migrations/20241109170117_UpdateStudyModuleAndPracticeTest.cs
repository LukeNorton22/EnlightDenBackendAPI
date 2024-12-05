using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnlightDenBackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStudyModuleAndPracticeTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PracticeTest_StudyModuleId",
                schema: "General",
                table: "PracticeTest");

            migrationBuilder.RenameColumn(
                name: "Request",
                schema: "General",
                table: "PracticeQuestion",
                newName: "Question");

            migrationBuilder.CreateIndex(
                name: "IX_PracticeTest_StudyModuleId",
                schema: "General",
                table: "PracticeTest",
                column: "StudyModuleId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PracticeTest_StudyModuleId",
                schema: "General",
                table: "PracticeTest");

            migrationBuilder.RenameColumn(
                name: "Question",
                schema: "General",
                table: "PracticeQuestion",
                newName: "Request");

            migrationBuilder.CreateIndex(
                name: "IX_PracticeTest_StudyModuleId",
                schema: "General",
                table: "PracticeTest",
                column: "StudyModuleId");
        }
    }
}
