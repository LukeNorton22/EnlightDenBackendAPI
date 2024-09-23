using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnlightDenBackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateClassEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classes_User_UserId",
                schema: "General",
                table: "Classes");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "General",
                table: "Classes",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_AspNetUsers_UserId",
                schema: "General",
                table: "Classes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classes_AspNetUsers_UserId",
                schema: "General",
                table: "Classes");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                schema: "General",
                table: "Classes",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_User_UserId",
                schema: "General",
                table: "Classes",
                column: "UserId",
                principalSchema: "Authorization",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
