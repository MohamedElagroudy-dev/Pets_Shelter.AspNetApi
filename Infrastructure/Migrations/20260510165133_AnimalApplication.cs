using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AnimalApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdoptionApplications_Animals_AnimalId",
                table: "AdoptionApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_AdoptionApplications_AspNetUsers_ApplicantId",
                table: "AdoptionApplications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AdoptionApplications",
                table: "AdoptionApplications");

            migrationBuilder.RenameTable(
                name: "AdoptionApplications",
                newName: "AnimalApplications");

            migrationBuilder.RenameIndex(
                name: "IX_AdoptionApplications_ApplicantId",
                table: "AnimalApplications",
                newName: "IX_AnimalApplications_ApplicantId");

            migrationBuilder.RenameIndex(
                name: "IX_AdoptionApplications_AnimalId",
                table: "AnimalApplications",
                newName: "IX_AnimalApplications_AnimalId");

            migrationBuilder.AddColumn<int>(
                name: "ApplicationType",
                table: "AnimalApplications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AnimalApplications",
                table: "AnimalApplications",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AnimalApplications_Animals_AnimalId",
                table: "AnimalApplications",
                column: "AnimalId",
                principalTable: "Animals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AnimalApplications_AspNetUsers_ApplicantId",
                table: "AnimalApplications",
                column: "ApplicantId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnimalApplications_Animals_AnimalId",
                table: "AnimalApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_AnimalApplications_AspNetUsers_ApplicantId",
                table: "AnimalApplications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AnimalApplications",
                table: "AnimalApplications");

            migrationBuilder.DropColumn(
                name: "ApplicationType",
                table: "AnimalApplications");

            migrationBuilder.RenameTable(
                name: "AnimalApplications",
                newName: "AdoptionApplications");

            migrationBuilder.RenameIndex(
                name: "IX_AnimalApplications_ApplicantId",
                table: "AdoptionApplications",
                newName: "IX_AdoptionApplications_ApplicantId");

            migrationBuilder.RenameIndex(
                name: "IX_AnimalApplications_AnimalId",
                table: "AdoptionApplications",
                newName: "IX_AdoptionApplications_AnimalId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AdoptionApplications",
                table: "AdoptionApplications",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AdoptionApplications_Animals_AnimalId",
                table: "AdoptionApplications",
                column: "AnimalId",
                principalTable: "Animals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AdoptionApplications_AspNetUsers_ApplicantId",
                table: "AdoptionApplications",
                column: "ApplicantId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
