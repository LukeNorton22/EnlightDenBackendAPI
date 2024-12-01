using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnlightDenBackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class StudyModuleNavPropertyInStudyTools : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudyModuleId",
                schema: "General",
                table: "StudyTools");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "StudyModuleId",
                schema: "General",
                table: "StudyTools",
                type: "uuid",
                nullable: true);
        }
    }
}
