using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnlightDenBackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class NoteIdToMindMap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "NoteId",
                schema: "General",
                table: "MindMaps",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NoteId",
                schema: "General",
                table: "MindMaps");
        }
    }
}
