using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_SubscriptionPlans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubscriptionPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DurationInMonths = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlanType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlanFeatures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FeatureText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubscriptionPlanId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanFeatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanFeatures_SubscriptionPlans_SubscriptionPlanId",
                        column: x => x.SubscriptionPlanId,
                        principalTable: "SubscriptionPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "SubscriptionPlans",
                columns: new[] { "Id", "Description", "DurationInMonths", "Name", "PlanType", "Price" },
                values: new object[,]
                {
                    { 1, null, 3, "Bronze Paws", 1, 300m },
                    { 2, null, 5, "Silver Whiskers", 1, 450m },
                    { 3, null, 7, "Golden Tails", 1, 600m },
                    { 4, null, 3, "Bronze Foster", 2, 300m },
                    { 5, null, 5, "Silver Foster", 2, 450m },
                    { 6, null, 7, "Golden Foster", 2, 600m },
                    { 7, null, 0, "Free Sponsor", 3, 0m }
                });

            migrationBuilder.InsertData(
                table: "PlanFeatures",
                columns: new[] { "Id", "FeatureText", "SubscriptionPlanId" },
                values: new object[,]
                {
                    { 1, "3 months unlimited profiles", 1 },
                    { 2, "2 Instagram & Facebook stories per month", 1 },
                    { 3, "5 months unlimited profiles", 2 },
                    { 4, "2 Instagram & Facebook stories per month", 2 },
                    { 5, "7 months unlimited profiles", 3 },
                    { 6, "2 Instagram & Facebook stories per month", 3 },
                    { 7, "Free consultation", 3 },
                    { 8, "3 months unlimited foster listings", 4 },
                    { 9, "2 social media stories per month", 4 },
                    { 10, "5 months unlimited foster listings", 5 },
                    { 11, "2 social media stories per month", 5 },
                    { 12, "7 months unlimited foster listings", 6 },
                    { 13, "2 social media stories per month", 6 },
                    { 14, "Homepage highlight", 6 },
                    { 15, "Upload case for sponsorship", 7 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlanFeatures_SubscriptionPlanId",
                table: "PlanFeatures",
                column: "SubscriptionPlanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlanFeatures");

            migrationBuilder.DropTable(
                name: "SubscriptionPlans");
        }
    }
}
