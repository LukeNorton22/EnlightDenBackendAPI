using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnlightDenBackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class StudyPlanTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Class_User_UserId",
                schema: "General",
                table: "Class"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_Notes_Class_ClassId",
                schema: "General",
                table: "Notes"
            );

            migrationBuilder.DropPrimaryKey(name: "PK_Class", schema: "General", table: "Class");

            migrationBuilder.RenameTable(
                name: "Class",
                schema: "General",
                newName: "Classes",
                newSchema: "General"
            );

            migrationBuilder.RenameIndex(
                name: "IX_Class_UserId",
                schema: "General",
                table: "Classes",
                newName: "IX_Classes_UserId"
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_Classes",
                schema: "General",
                table: "Classes",
                column: "Id"
            );

            migrationBuilder.CreateTable(
                name: "StudyPlans",
                schema: "General",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(
                        type: "character varying(500)",
                        maxLength: 500,
                        nullable: true
                    ),
                    Day = table.Column<int>(type: "integer", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    StartTime = table.Column<long>(type: "bigint", nullable: false),
                    EndTime = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyPlans_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "Authorization",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_StudyPlans_UserId",
                schema: "General",
                table: "StudyPlans",
                column: "UserId"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_User_UserId",
                schema: "General",
                table: "Classes",
                column: "UserId",
                principalSchema: "Authorization",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_Classes_ClassId",
                schema: "General",
                table: "Notes",
                column: "ClassId",
                principalSchema: "General",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classes_User_UserId",
                schema: "General",
                table: "Classes"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_Notes_Classes_ClassId",
                schema: "General",
                table: "Notes"
            );

            migrationBuilder.DropTable(name: "StudyPlans", schema: "General");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Classes",
                schema: "General",
                table: "Classes"
            );

            migrationBuilder.RenameTable(
                name: "Classes",
                schema: "General",
                newName: "Class",
                newSchema: "General"
            );

            migrationBuilder.RenameIndex(
                name: "IX_Classes_UserId",
                schema: "General",
                table: "Class",
                newName: "IX_Class_UserId"
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_Class",
                schema: "General",
                table: "Class",
                column: "Id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Class_User_UserId",
                schema: "General",
                table: "Class",
                column: "UserId",
                principalSchema: "Authorization",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_Class_ClassId",
                schema: "General",
                table: "Notes",
                column: "ClassId",
                principalSchema: "General",
                principalTable: "Class",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }
    }
}
