using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFosteTableAndAnimalRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AnimalRole",
                table: "Animals",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "Adoption");

            migrationBuilder.AddColumn<DateTime>(
                name: "FosterEndDate",
                table: "Animals",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FosterNotes",
                table: "Animals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FosterStartDate",
                table: "Animals",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FostererId",
                table: "Animals",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Animals_FostererId",
                table: "Animals",
                column: "FostererId");

            migrationBuilder.AddForeignKey(
                name: "FK_Animals_AspNetUsers_FostererId",
                table: "Animals",
                column: "FostererId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Animals_AspNetUsers_FostererId",
                table: "Animals");

            migrationBuilder.DropIndex(
                name: "IX_Animals_FostererId",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "AnimalRole",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "FosterEndDate",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "FosterNotes",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "FosterStartDate",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "FostererId",
                table: "Animals");
        }
    }
}
