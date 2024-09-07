using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnlightDenBackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStudyToolsTableAndQuestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_StudyTools_StudyToolId",
                schema: "General",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "QuestionIds",
                schema: "General",
                table: "StudyTools");

            migrationBuilder.AlterColumn<Guid>(
                name: "StudyToolId",
                schema: "General",
                table: "Questions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_StudyTools_StudyToolId",
                schema: "General",
                table: "Questions",
                column: "StudyToolId",
                principalSchema: "General",
                principalTable: "StudyTools",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_StudyTools_StudyToolId",
                schema: "General",
                table: "Questions");

            migrationBuilder.AddColumn<List<Guid>>(
                name: "QuestionIds",
                schema: "General",
                table: "StudyTools",
                type: "uuid[]",
                nullable: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "StudyToolId",
                schema: "General",
                table: "Questions",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_StudyTools_StudyToolId",
                schema: "General",
                table: "Questions",
                column: "StudyToolId",
                principalSchema: "General",
                principalTable: "StudyTools",
                principalColumn: "Id");
        }
    }
}
