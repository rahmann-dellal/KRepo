using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KFP.Migrations
{
    /// <inheritdoc />
    public partial class RenamingInvoiceToPaymentReceipt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InvoiceId",
                table: "Invoices",
                newName: "PaymentReceiptId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentReceiptId",
                table: "Invoices",
                newName: "InvoiceId");
        }
    }
}
