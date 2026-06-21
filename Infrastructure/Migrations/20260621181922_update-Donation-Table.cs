using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateDonationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DonorName",
                table: "Donations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DonorProfilePicture",
                table: "Donations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAnonymous",
                table: "Donations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidAt",
                table: "Donations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentStatus",
                table: "Donations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "StripePaymentIntentId",
                table: "Donations",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DonorName",
                table: "Donations");

            migrationBuilder.DropColumn(
                name: "DonorProfilePicture",
                table: "Donations");

            migrationBuilder.DropColumn(
                name: "IsAnonymous",
                table: "Donations");

            migrationBuilder.DropColumn(
                name: "PaidAt",
                table: "Donations");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Donations");

            migrationBuilder.DropColumn(
                name: "StripePaymentIntentId",
                table: "Donations");
        }
    }
}
