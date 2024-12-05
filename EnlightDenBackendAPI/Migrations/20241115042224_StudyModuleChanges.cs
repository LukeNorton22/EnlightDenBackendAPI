using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnlightDenBackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class StudyModuleChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PracticeQuestion",
                schema: "General");

            migrationBuilder.DropTable(
                name: "PracticeTest",
                schema: "General");

            migrationBuilder.RenameColumn(
                name: "MindMapId",
                schema: "General",
                table: "StudyModule",
                newName: "MindMapTopicId");

            migrationBuilder.RenameColumn(
                name: "MainTopic",
                schema: "General",
                table: "StudyModule",
                newName: "MindMapTopicName");

            migrationBuilder.AddColumn<Guid>(
                name: "MindMapTopicId",
                schema: "General",
                table: "SubTopic",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_StudyModule_MindMapTopicId",
                schema: "General",
                table: "StudyModule",
                column: "MindMapTopicId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudyModule_MindMapTopics_MindMapTopicId",
                schema: "General",
                table: "StudyModule",
                column: "MindMapTopicId",
                principalSchema: "General",
                principalTable: "MindMapTopics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudyModule_MindMapTopics_MindMapTopicId",
                schema: "General",
                table: "StudyModule");

            migrationBuilder.DropIndex(
                name: "IX_StudyModule_MindMapTopicId",
                schema: "General",
                table: "StudyModule");

            migrationBuilder.DropColumn(
                name: "MindMapTopicId",
                schema: "General",
                table: "SubTopic");

            migrationBuilder.RenameColumn(
                name: "MindMapTopicName",
                schema: "General",
                table: "StudyModule",
                newName: "MainTopic");

            migrationBuilder.RenameColumn(
                name: "MindMapTopicId",
                schema: "General",
                table: "StudyModule",
                newName: "MindMapId");

            migrationBuilder.CreateTable(
                name: "PracticeTest",
                schema: "General",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudyModuleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PracticeTest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PracticeTest_StudyModule_StudyModuleId",
                        column: x => x.StudyModuleId,
                        principalSchema: "General",
                        principalTable: "StudyModule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PracticeQuestion",
                schema: "General",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PracticeTestId = table.Column<Guid>(type: "uuid", nullable: false),
                    Answer = table.Column<string>(type: "text", nullable: false),
                    Question = table.Column<string>(type: "text", nullable: false),
                    QuestionType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PracticeQuestion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PracticeQuestion_PracticeTest_PracticeTestId",
                        column: x => x.PracticeTestId,
                        principalSchema: "General",
                        principalTable: "PracticeTest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PracticeQuestion_PracticeTestId",
                schema: "General",
                table: "PracticeQuestion",
                column: "PracticeTestId");

            migrationBuilder.CreateIndex(
                name: "IX_PracticeTest_StudyModuleId",
                schema: "General",
                table: "PracticeTest",
                column: "StudyModuleId",
                unique: true);
        }
    }
}
