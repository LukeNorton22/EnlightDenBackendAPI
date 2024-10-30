using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnlightDenBackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class StudyModuleToStudyTools : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PracticeTest_StudyModules_StudyModuleId",
                table: "PracticeTest");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyModules_StudyTools_StudyToolId",
                table: "StudyModules");

            migrationBuilder.DropForeignKey(
                name: "FK_SubTopic_StudyModules_StudyModuleId",
                table: "SubTopic");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudyModules",
                table: "StudyModules");

            migrationBuilder.RenameTable(
                name: "SubTopic",
                newName: "SubTopic",
                newSchema: "General");

            migrationBuilder.RenameTable(
                name: "PracticeTest",
                newName: "PracticeTest",
                newSchema: "General");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                newName: "AspNetUserTokens",
                newSchema: "General");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                newName: "AspNetUsers",
                newSchema: "General");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                newName: "AspNetUserRoles",
                newSchema: "General");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                newName: "AspNetUserLogins",
                newSchema: "General");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                newName: "AspNetUserClaims",
                newSchema: "General");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                newName: "AspNetRoles",
                newSchema: "General");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                newName: "AspNetRoleClaims",
                newSchema: "General");

            migrationBuilder.RenameTable(
                name: "StudyModules",
                newName: "StudyModule",
                newSchema: "General");

            migrationBuilder.RenameIndex(
                name: "IX_StudyModules_StudyToolId",
                schema: "General",
                table: "StudyModule",
                newName: "IX_StudyModule_StudyToolId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudyModule",
                schema: "General",
                table: "StudyModule",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PracticeTest_StudyModule_StudyModuleId",
                schema: "General",
                table: "PracticeTest",
                column: "StudyModuleId",
                principalSchema: "General",
                principalTable: "StudyModule",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudyModule_StudyTools_StudyToolId",
                schema: "General",
                table: "StudyModule",
                column: "StudyToolId",
                principalSchema: "General",
                principalTable: "StudyTools",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubTopic_StudyModule_StudyModuleId",
                schema: "General",
                table: "SubTopic",
                column: "StudyModuleId",
                principalSchema: "General",
                principalTable: "StudyModule",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PracticeTest_StudyModule_StudyModuleId",
                schema: "General",
                table: "PracticeTest");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyModule_StudyTools_StudyToolId",
                schema: "General",
                table: "StudyModule");

            migrationBuilder.DropForeignKey(
                name: "FK_SubTopic_StudyModule_StudyModuleId",
                schema: "General",
                table: "SubTopic");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudyModule",
                schema: "General",
                table: "StudyModule");

            migrationBuilder.RenameTable(
                name: "SubTopic",
                schema: "General",
                newName: "SubTopic");

            migrationBuilder.RenameTable(
                name: "PracticeTest",
                schema: "General",
                newName: "PracticeTest");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                schema: "General",
                newName: "AspNetUserTokens");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                schema: "General",
                newName: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                schema: "General",
                newName: "AspNetUserRoles");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                schema: "General",
                newName: "AspNetUserLogins");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                schema: "General",
                newName: "AspNetUserClaims");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                schema: "General",
                newName: "AspNetRoles");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                schema: "General",
                newName: "AspNetRoleClaims");

            migrationBuilder.RenameTable(
                name: "StudyModule",
                schema: "General",
                newName: "StudyModules");

            migrationBuilder.RenameIndex(
                name: "IX_StudyModule_StudyToolId",
                table: "StudyModules",
                newName: "IX_StudyModules_StudyToolId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudyModules",
                table: "StudyModules",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PracticeTest_StudyModules_StudyModuleId",
                table: "PracticeTest",
                column: "StudyModuleId",
                principalTable: "StudyModules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudyModules_StudyTools_StudyToolId",
                table: "StudyModules",
                column: "StudyToolId",
                principalSchema: "General",
                principalTable: "StudyTools",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubTopic_StudyModules_StudyModuleId",
                table: "SubTopic",
                column: "StudyModuleId",
                principalTable: "StudyModules",
                principalColumn: "Id");
        }
    }
}
