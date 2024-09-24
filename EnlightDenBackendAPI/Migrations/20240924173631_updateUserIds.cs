using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnlightDenBackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class updateUserIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MindMaps_User_UserId",
                schema: "General",
                table: "MindMaps");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyPlans_User_UserId",
                schema: "General",
                table: "StudyPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyTools_User_UserId",
                schema: "General",
                table: "StudyTools");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "General",
                table: "StudyTools",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "General",
                table: "StudyPlans",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "General",
                table: "MindMaps",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_MindMaps_AspNetUsers_UserId",
                schema: "General",
                table: "MindMaps",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudyPlans_AspNetUsers_UserId",
                schema: "General",
                table: "StudyPlans",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudyTools_AspNetUsers_UserId",
                schema: "General",
                table: "StudyTools",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MindMaps_AspNetUsers_UserId",
                schema: "General",
                table: "MindMaps");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyPlans_AspNetUsers_UserId",
                schema: "General",
                table: "StudyPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyTools_AspNetUsers_UserId",
                schema: "General",
                table: "StudyTools");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                schema: "General",
                table: "StudyTools",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                schema: "General",
                table: "StudyPlans",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                schema: "General",
                table: "MindMaps",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_MindMaps_User_UserId",
                schema: "General",
                table: "MindMaps",
                column: "UserId",
                principalSchema: "Authorization",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudyPlans_User_UserId",
                schema: "General",
                table: "StudyPlans",
                column: "UserId",
                principalSchema: "Authorization",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudyTools_User_UserId",
                schema: "General",
                table: "StudyTools",
                column: "UserId",
                principalSchema: "Authorization",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
