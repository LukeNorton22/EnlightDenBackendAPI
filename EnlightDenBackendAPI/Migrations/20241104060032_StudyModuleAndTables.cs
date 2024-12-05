using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnlightDenBackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class StudyModuleAndTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "StudyModuleId",
                schema: "General",
                table: "StudyTools",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StudyModule",
                schema: "General",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MainTopic = table.Column<string>(type: "text", nullable: false),
                    StudyToolId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyModule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyModule_StudyTools_StudyToolId",
                        column: x => x.StudyToolId,
                        principalSchema: "General",
                        principalTable: "StudyTools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "SubTopic",
                schema: "General",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    StudyModuleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubTopic", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubTopic_StudyModule_StudyModuleId",
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
                    Request = table.Column<string>(type: "text", nullable: false),
                    Answer = table.Column<string>(type: "text", nullable: false),
                    QuestionType = table.Column<int>(type: "integer", nullable: false),
                    PracticeTestId = table.Column<Guid>(type: "uuid", nullable: false)
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
                column: "StudyModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyModule_StudyToolId",
                schema: "General",
                table: "StudyModule",
                column: "StudyToolId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubTopic_StudyModuleId",
                schema: "General",
                table: "SubTopic",
                column: "StudyModuleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PracticeQuestion",
                schema: "General");

            migrationBuilder.DropTable(
                name: "SubTopic",
                schema: "General");

            migrationBuilder.DropTable(
                name: "PracticeTest",
                schema: "General");

            migrationBuilder.DropTable(
                name: "StudyModule",
                schema: "General");

            migrationBuilder.DropColumn(
                name: "StudyModuleId",
                schema: "General",
                table: "StudyTools");
        }
    }
}
