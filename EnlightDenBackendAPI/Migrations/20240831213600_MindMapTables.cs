using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnlightDenBackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class MindMapTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MindMaps",
                schema: "General",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TopicIds = table.Column<List<Guid>>(type: "uuid[]", nullable: false),
                    ClassId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MindMaps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MindMaps_Classes_ClassId",
                        column: x => x.ClassId,
                        principalSchema: "General",
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MindMaps_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "Authorization",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MindMapTopics",
                schema: "General",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MindMapId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MindMapTopics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MindMapTopics_MindMaps_MindMapId",
                        column: x => x.MindMapId,
                        principalSchema: "General",
                        principalTable: "MindMaps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MindMaps_ClassId",
                schema: "General",
                table: "MindMaps",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_MindMaps_UserId",
                schema: "General",
                table: "MindMaps",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MindMapTopics_MindMapId",
                schema: "General",
                table: "MindMapTopics",
                column: "MindMapId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MindMapTopics",
                schema: "General");

            migrationBuilder.DropTable(
                name: "MindMaps",
                schema: "General");
        }
    }
}
