using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KFP.Migrations
{
    /// <inheritdoc />
    public partial class addingSessionToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isPreparing",
                table: "Orders",
                newName: "SetReadyAt");

            migrationBuilder.RenameColumn(
                name: "ServedAt",
                table: "Orders",
                newName: "SetPreparingAt");

            migrationBuilder.AddColumn<int>(
                name: "SessionId",
                table: "Orders",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_SessionId",
                table: "Orders",
                column: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Sessions_SessionId",
                table: "Orders",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "SessionId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Sessions_SessionId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_SessionId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "SetReadyAt",
                table: "Orders",
                newName: "isPreparing");

            migrationBuilder.RenameColumn(
                name: "SetPreparingAt",
                table: "Orders",
                newName: "ServedAt");
        }
    }
}
