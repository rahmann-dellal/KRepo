using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KFP.Migrations
{
    /// <inheritdoc />
    public partial class RenamingInvoiceToPaymentReceipt3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_AppUsers_AppUserId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Orders_OrderId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Sessions_SessionId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Invoices_InvoiceId",
                table: "Sales");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Invoices",
                table: "Invoices");

            migrationBuilder.RenameTable(
                name: "Invoices",
                newName: "PaymentReceipts");

            migrationBuilder.RenameIndex(
                name: "IX_Invoices_SessionId",
                table: "PaymentReceipts",
                newName: "IX_PaymentReceipts_SessionId");

            migrationBuilder.RenameIndex(
                name: "IX_Invoices_OrderId",
                table: "PaymentReceipts",
                newName: "IX_PaymentReceipts_OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_Invoices_AppUserId",
                table: "PaymentReceipts",
                newName: "IX_PaymentReceipts_AppUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentReceipts",
                table: "PaymentReceipts",
                column: "PaymentReceiptId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentReceipts_AppUsers_AppUserId",
                table: "PaymentReceipts",
                column: "AppUserId",
                principalTable: "AppUsers",
                principalColumn: "AppUserID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentReceipts_Orders_OrderId",
                table: "PaymentReceipts",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentReceipts_Sessions_SessionId",
                table: "PaymentReceipts",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_PaymentReceipts_InvoiceId",
                table: "Sales",
                column: "InvoiceId",
                principalTable: "PaymentReceipts",
                principalColumn: "PaymentReceiptId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentReceipts_AppUsers_AppUserId",
                table: "PaymentReceipts");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentReceipts_Orders_OrderId",
                table: "PaymentReceipts");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentReceipts_Sessions_SessionId",
                table: "PaymentReceipts");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_PaymentReceipts_InvoiceId",
                table: "Sales");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentReceipts",
                table: "PaymentReceipts");

            migrationBuilder.RenameTable(
                name: "PaymentReceipts",
                newName: "Invoices");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentReceipts_SessionId",
                table: "Invoices",
                newName: "IX_Invoices_SessionId");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentReceipts_OrderId",
                table: "Invoices",
                newName: "IX_Invoices_OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentReceipts_AppUserId",
                table: "Invoices",
                newName: "IX_Invoices_AppUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Invoices",
                table: "Invoices",
                column: "PaymentReceiptId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_AppUsers_AppUserId",
                table: "Invoices",
                column: "AppUserId",
                principalTable: "AppUsers",
                principalColumn: "AppUserID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Orders_OrderId",
                table: "Invoices",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Sessions_SessionId",
                table: "Invoices",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Invoices_InvoiceId",
                table: "Sales",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "PaymentReceiptId",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
