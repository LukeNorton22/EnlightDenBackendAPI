using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnlightDenBackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class NotesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classes_User_UserId",
                table: "Classes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Classes",
                table: "Classes");

            migrationBuilder.EnsureSchema(
                name: "General");

            migrationBuilder.RenameTable(
                name: "Classes",
                newName: "Class",
                newSchema: "General");

            migrationBuilder.RenameIndex(
                name: "IX_Classes_UserId",
                schema: "General",
                table: "Class",
                newName: "IX_Class_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Class",
                schema: "General",
                table: "Class",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Notes",
                schema: "General",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    CreateDate = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClassId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notes_Class_ClassId",
                        column: x => x.ClassId,
                        principalSchema: "General",
                        principalTable: "Class",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notes_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "Authorization",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notes_ClassId",
                schema: "General",
                table: "Notes",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_UserId",
                schema: "General",
                table: "Notes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Class_User_UserId",
                schema: "General",
                table: "Class",
                column: "UserId",
                principalSchema: "Authorization",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Class_User_UserId",
                schema: "General",
                table: "Class");

            migrationBuilder.DropTable(
                name: "Notes",
                schema: "General");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Class",
                schema: "General",
                table: "Class");

            migrationBuilder.RenameTable(
                name: "Class",
                schema: "General",
                newName: "Classes");

            migrationBuilder.RenameIndex(
                name: "IX_Class_UserId",
                table: "Classes",
                newName: "IX_Classes_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Classes",
                table: "Classes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_User_UserId",
                table: "Classes",
                column: "UserId",
                principalSchema: "Authorization",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
