using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAdoptionApplications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdoptionApplications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnimalId = table.Column<int>(type: "int", nullable: false),
                    ApplicantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ApplicantInfo_FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ApplicantInfo_LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ApplicantInfo_PhoneNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ApplicantInfo_Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    AddressInfo_Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AddressInfo_City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AddressInfo_ZipCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AddressInfo_Address = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    HouseholdInfo_Details = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    PetCareInfo_ResponsiblePerson = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    PetCareInfo_AdoptionReason = table.Column<string>(type: "nvarchar(max)", maxLength: 10000, nullable: false),
                    PetCareInfo_AloneTimeDetails = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    PetCareInfo_LivingEnvironment = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: false),
                    Preferences_Dog = table.Column<bool>(type: "bit", nullable: false),
                    Preferences_Cat = table.Column<bool>(type: "bit", nullable: false),
                    Preferences_Bird = table.Column<bool>(type: "bit", nullable: false),
                    Preferences_Lizard = table.Column<bool>(type: "bit", nullable: false),
                    Preferences_Rabbit = table.Column<bool>(type: "bit", nullable: false),
                    Preferences_Other = table.Column<bool>(type: "bit", nullable: false),
                    Preferences_HouseTrained = table.Column<bool>(type: "bit", nullable: false),
                    Preferences_Declawed = table.Column<bool>(type: "bit", nullable: false),
                    Preferences_Young = table.Column<bool>(type: "bit", nullable: false),
                    Preferences_MultiplePets = table.Column<bool>(type: "bit", nullable: false),
                    Preferences_SpecialConsiderations = table.Column<bool>(type: "bit", nullable: false),
                    Agreement_Accepted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AdminNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdoptionApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdoptionApplications_Animals_AnimalId",
                        column: x => x.AnimalId,
                        principalTable: "Animals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AdoptionApplications_AspNetUsers_ApplicantId",
                        column: x => x.ApplicantId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdoptionApplications_AnimalId",
                table: "AdoptionApplications",
                column: "AnimalId");

            migrationBuilder.CreateIndex(
                name: "IX_AdoptionApplications_ApplicantId",
                table: "AdoptionApplications",
                column: "ApplicantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdoptionApplications");
        }
    }
}
