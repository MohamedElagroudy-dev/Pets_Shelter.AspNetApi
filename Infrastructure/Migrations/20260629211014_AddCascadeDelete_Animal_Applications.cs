using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCascadeDelete_Animal_Applications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnimalApplications_Animals_AnimalId",
                table: "AnimalApplications");

            migrationBuilder.AddForeignKey(
                name: "FK_AnimalApplications_Animals_AnimalId",
                table: "AnimalApplications",
                column: "AnimalId",
                principalTable: "Animals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnimalApplications_Animals_AnimalId",
                table: "AnimalApplications");

            migrationBuilder.AddForeignKey(
                name: "FK_AnimalApplications_Animals_AnimalId",
                table: "AnimalApplications",
                column: "AnimalId",
                principalTable: "Animals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
