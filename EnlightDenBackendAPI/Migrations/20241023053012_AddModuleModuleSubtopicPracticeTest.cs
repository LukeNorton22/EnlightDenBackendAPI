using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnlightDenBackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddModuleModuleSubtopicPracticeTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "ModuleSubtopics",
                newName: "ModuleSubtopics",
                newSchema: "General");

            migrationBuilder.RenameTable(
                name: "Modules",
                newName: "Modules",
                newSchema: "General");

            migrationBuilder.AddColumn<Guid>(
                name: "PracticeTestId",
                schema: "General",
                table: "Questions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "General",
                table: "ModuleSubtopics",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<List<Guid>>(
                name: "SubtopicIds",
                schema: "General",
                table: "Modules",
                type: "uuid[]",
                nullable: false);

            migrationBuilder.CreateTable(
                name: "PracticeTests",
                schema: "General",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ModuleId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PracticeTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PracticeTests_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalSchema: "General",
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Questions_PracticeTestId",
                schema: "General",
                table: "Questions",
                column: "PracticeTestId");

            migrationBuilder.CreateIndex(
                name: "IX_Modules_NoteId",
                schema: "General",
                table: "Modules",
                column: "NoteId");

            migrationBuilder.CreateIndex(
                name: "IX_PracticeTests_ModuleId",
                schema: "General",
                table: "PracticeTests",
                column: "ModuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Modules_Notes_NoteId",
                schema: "General",
                table: "Modules",
                column: "NoteId",
                principalSchema: "General",
                principalTable: "Notes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_PracticeTests_PracticeTestId",
                schema: "General",
                table: "Questions",
                column: "PracticeTestId",
                principalSchema: "General",
                principalTable: "PracticeTests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Modules_Notes_NoteId",
                schema: "General",
                table: "Modules");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_PracticeTests_PracticeTestId",
                schema: "General",
                table: "Questions");

            migrationBuilder.DropTable(
                name: "PracticeTests",
                schema: "General");

            migrationBuilder.DropIndex(
                name: "IX_Questions_PracticeTestId",
                schema: "General",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Modules_NoteId",
                schema: "General",
                table: "Modules");

            migrationBuilder.DropColumn(
                name: "PracticeTestId",
                schema: "General",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "SubtopicIds",
                schema: "General",
                table: "Modules");

            migrationBuilder.RenameTable(
                name: "ModuleSubtopics",
                schema: "General",
                newName: "ModuleSubtopics");

            migrationBuilder.RenameTable(
                name: "Modules",
                schema: "General",
                newName: "Modules");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ModuleSubtopics",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);
        }
    }
}
