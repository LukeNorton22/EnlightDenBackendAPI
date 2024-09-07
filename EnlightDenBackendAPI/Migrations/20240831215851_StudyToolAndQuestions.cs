using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnlightDenBackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class StudyToolAndQuestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudyTools",
                schema: "General",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionIds = table.Column<List<Guid>>(type: "uuid[]", nullable: false),
                    ClassId = table.Column<Guid>(type: "uuid", nullable: false),
                    MindMapTopicId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContentType = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyTools", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyTools_Classes_ClassId",
                        column: x => x.ClassId,
                        principalSchema: "General",
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_StudyTools_MindMapTopics_MindMapTopicId",
                        column: x => x.MindMapTopicId,
                        principalSchema: "General",
                        principalTable: "MindMapTopics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_StudyTools_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "Authorization",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Questions",
                schema: "General",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Request = table.Column<string>(type: "text", nullable: false),
                    Answer = table.Column<string>(type: "text", nullable: false),
                    QuestionType = table.Column<int>(type: "integer", nullable: false),
                    ClassId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudyToolId = table.Column<Guid>(type: "uuid", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_Classes_ClassId",
                        column: x => x.ClassId,
                        principalSchema: "General",
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_Questions_StudyTools_StudyToolId",
                        column: x => x.StudyToolId,
                        principalSchema: "General",
                        principalTable: "StudyTools",
                        principalColumn: "Id"
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Questions_ClassId",
                schema: "General",
                table: "Questions",
                column: "ClassId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Questions_StudyToolId",
                schema: "General",
                table: "Questions",
                column: "StudyToolId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_StudyTools_ClassId",
                schema: "General",
                table: "StudyTools",
                column: "ClassId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_StudyTools_MindMapTopicId",
                schema: "General",
                table: "StudyTools",
                column: "MindMapTopicId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_StudyTools_UserId",
                schema: "General",
                table: "StudyTools",
                column: "UserId"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Questions", schema: "General");

            migrationBuilder.DropTable(name: "StudyTools", schema: "General");
        }
    }
}
