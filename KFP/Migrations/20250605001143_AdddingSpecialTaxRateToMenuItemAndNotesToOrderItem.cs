using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KFP.Migrations
{
    /// <inheritdoc />
    public partial class AdddingSpecialTaxRateToMenuItemAndNotesToOrderItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "OrderItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "SpecialTaxRate",
                table: "MenuItems",
                type: "REAL",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "SpecialTaxRate",
                table: "MenuItems");
        }
    }
}
