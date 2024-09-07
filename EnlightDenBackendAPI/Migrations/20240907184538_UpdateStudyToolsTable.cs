using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnlightDenBackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStudyToolsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudyTools_MindMapTopics_MindMapTopicId",
                schema: "General",
                table: "StudyTools");

            migrationBuilder.RenameColumn(
                name: "MindMapTopicId",
                schema: "General",
                table: "StudyTools",
                newName: "MindMapId");

            migrationBuilder.RenameIndex(
                name: "IX_StudyTools_MindMapTopicId",
                schema: "General",
                table: "StudyTools",
                newName: "IX_StudyTools_MindMapId");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "General",
                table: "StudyTools",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_StudyTools_MindMaps_MindMapId",
                schema: "General",
                table: "StudyTools",
                column: "MindMapId",
                principalSchema: "General",
                principalTable: "MindMaps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudyTools_MindMaps_MindMapId",
                schema: "General",
                table: "StudyTools");

            migrationBuilder.DropColumn(
                name: "Name",
                schema: "General",
                table: "StudyTools");

            migrationBuilder.RenameColumn(
                name: "MindMapId",
                schema: "General",
                table: "StudyTools",
                newName: "MindMapTopicId");

            migrationBuilder.RenameIndex(
                name: "IX_StudyTools_MindMapId",
                schema: "General",
                table: "StudyTools",
                newName: "IX_StudyTools_MindMapTopicId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudyTools_MindMapTopics_MindMapTopicId",
                schema: "General",
                table: "StudyTools",
                column: "MindMapTopicId",
                principalSchema: "General",
                principalTable: "MindMapTopics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
