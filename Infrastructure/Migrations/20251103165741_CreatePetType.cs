using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreatePetType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PetTypeId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QuantityInStock",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PetTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_PetTypeId",
                table: "Products",
                column: "PetTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_PetTypes_PetTypeId",
                table: "Products",
                column: "PetTypeId",
                principalTable: "PetTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_PetTypes_PetTypeId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "PetTypes");

            migrationBuilder.DropIndex(
                name: "IX_Products_PetTypeId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PetTypeId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "QuantityInStock",
                table: "Products");
        }
    }
}
