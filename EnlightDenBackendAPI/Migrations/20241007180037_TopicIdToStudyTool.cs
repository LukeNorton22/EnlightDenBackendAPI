﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnlightDenBackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class TopicIdToStudyTool : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TopicId",
                schema: "General",
                table: "StudyTools",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TopicId",
                schema: "General",
                table: "StudyTools");
        }
    }
}
