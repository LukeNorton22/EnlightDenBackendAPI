﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnlightDenBackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class ClassTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "Authorization",
                table: "User",
                type: "text",
                nullable: true
            );

            migrationBuilder.CreateTable(
                name: "Classes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Classes_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "Authorization",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Classes_UserId",
                table: "Classes",
                column: "UserId"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Classes");

            migrationBuilder.DropColumn(name: "Email", schema: "Authorization", table: "User");
        }
    }
}
