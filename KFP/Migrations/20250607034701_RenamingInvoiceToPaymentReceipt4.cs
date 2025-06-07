using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KFP.Migrations
{
    /// <inheritdoc />
    public partial class RenamingInvoiceToPaymentReceipt4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales_PaymentReceipts_InvoiceId",
                table: "Sales");

            migrationBuilder.RenameColumn(
                name: "InvoiceId",
                table: "Sales",
                newName: "ReceiptId");

            migrationBuilder.RenameIndex(
                name: "IX_Sales_InvoiceId",
                table: "Sales",
                newName: "IX_Sales_ReceiptId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_PaymentReceipts_ReceiptId",
                table: "Sales",
                column: "ReceiptId",
                principalTable: "PaymentReceipts",
                principalColumn: "PaymentReceiptId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales_PaymentReceipts_ReceiptId",
                table: "Sales");

            migrationBuilder.RenameColumn(
                name: "ReceiptId",
                table: "Sales",
                newName: "InvoiceId");

            migrationBuilder.RenameIndex(
                name: "IX_Sales_ReceiptId",
                table: "Sales",
                newName: "IX_Sales_InvoiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_PaymentReceipts_InvoiceId",
                table: "Sales",
                column: "InvoiceId",
                principalTable: "PaymentReceipts",
                principalColumn: "PaymentReceiptId",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
