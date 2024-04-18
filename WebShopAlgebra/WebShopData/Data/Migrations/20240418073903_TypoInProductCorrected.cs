using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebShopData.Data.Migrations
{
    /// <inheritdoc />
    public partial class TypoInProductCorrected : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PriceMoreThen3",
                table: "Product",
                newName: "PriceMoreThan3");

            migrationBuilder.RenameColumn(
                name: "PriceMoreThen10",
                table: "Product",
                newName: "PriceMoreThan10");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PriceMoreThan3",
                table: "Product",
                newName: "PriceMoreThen3");

            migrationBuilder.RenameColumn(
                name: "PriceMoreThan10",
                table: "Product",
                newName: "PriceMoreThen10");
        }
    }
}
