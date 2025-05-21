using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KFP.Migrations
{
    /// <inheritdoc />
    public partial class addingSessionToInvoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ClosingCash",
                table: "Sessions",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "OpeningCash",
                table: "Sessions",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalCardPayments",
                table: "Sessions",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "SessionId",
                table: "Invoices",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_SessionId",
                table: "Invoices",
                column: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Sessions_SessionId",
                table: "Invoices",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "SessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Sessions_SessionId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_SessionId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "ClosingCash",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "OpeningCash",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "TotalCardPayments",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "Invoices");
        }
    }
}
