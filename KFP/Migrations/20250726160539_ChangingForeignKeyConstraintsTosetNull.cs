using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KFP.Migrations
{
    /// <inheritdoc />
    public partial class ChangingForeignKeyConstraintsTosetNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentReceipts_Sessions_SessionId",
                table: "PaymentReceipts");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_AppUsers_AppUserID",
                table: "Sessions");

            migrationBuilder.AlterColumn<int>(
                name: "AppUserID",
                table: "Sessions",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentReceipts_Sessions_SessionId",
                table: "PaymentReceipts",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "SessionId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_AppUsers_AppUserID",
                table: "Sessions",
                column: "AppUserID",
                principalTable: "AppUsers",
                principalColumn: "AppUserID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentReceipts_Sessions_SessionId",
                table: "PaymentReceipts");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_AppUsers_AppUserID",
                table: "Sessions");

            migrationBuilder.AlterColumn<int>(
                name: "AppUserID",
                table: "Sessions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentReceipts_Sessions_SessionId",
                table: "PaymentReceipts",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_AppUsers_AppUserID",
                table: "Sessions",
                column: "AppUserID",
                principalTable: "AppUsers",
                principalColumn: "AppUserID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
