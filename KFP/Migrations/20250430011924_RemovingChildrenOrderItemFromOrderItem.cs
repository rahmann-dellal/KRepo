﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KFP.Migrations
{
    /// <inheritdoc />
    public partial class RemovingChildrenOrderItemFromOrderItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_OrderItems_OrderItemId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_OrderItemId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "OrderItemId",
                table: "OrderItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderItemId",
                table: "OrderItems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderItemId",
                table: "OrderItems",
                column: "OrderItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_OrderItems_OrderItemId",
                table: "OrderItems",
                column: "OrderItemId",
                principalTable: "OrderItems",
                principalColumn: "Id");
        }
    }
}
