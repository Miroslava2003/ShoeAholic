using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoeAholic.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrdersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Shoes_ShoeId",
                table: "OrderItems");

            migrationBuilder.AddColumn<int>(
                name: "ShoeId1",
                table: "ShoppingCartItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShoeSizeId",
                table: "ShoppingCartItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PaymentTransactionId",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCartItems_ShoeId",
                table: "ShoppingCartItems",
                column: "ShoeId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCartItems_ShoeId1",
                table: "ShoppingCartItems",
                column: "ShoeId1");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCartItems_ShoeSizeId",
                table: "ShoppingCartItems",
                column: "ShoeSizeId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCartItems_SizeId",
                table: "ShoppingCartItems",
                column: "SizeId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Shoes_ShoeId",
                table: "OrderItems",
                column: "ShoeId",
                principalTable: "Shoes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCartItems_ShoeSizes_ShoeSizeId",
                table: "ShoppingCartItems",
                column: "ShoeSizeId",
                principalTable: "ShoeSizes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCartItems_ShoeSizes_SizeId",
                table: "ShoppingCartItems",
                column: "SizeId",
                principalTable: "ShoeSizes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCartItems_Shoes_ShoeId",
                table: "ShoppingCartItems",
                column: "ShoeId",
                principalTable: "Shoes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCartItems_Shoes_ShoeId1",
                table: "ShoppingCartItems",
                column: "ShoeId1",
                principalTable: "Shoes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Shoes_ShoeId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCartItems_ShoeSizes_ShoeSizeId",
                table: "ShoppingCartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCartItems_ShoeSizes_SizeId",
                table: "ShoppingCartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCartItems_Shoes_ShoeId",
                table: "ShoppingCartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCartItems_Shoes_ShoeId1",
                table: "ShoppingCartItems");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingCartItems_ShoeId",
                table: "ShoppingCartItems");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingCartItems_ShoeId1",
                table: "ShoppingCartItems");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingCartItems_ShoeSizeId",
                table: "ShoppingCartItems");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingCartItems_SizeId",
                table: "ShoppingCartItems");

            migrationBuilder.DropColumn(
                name: "ShoeId1",
                table: "ShoppingCartItems");

            migrationBuilder.DropColumn(
                name: "ShoeSizeId",
                table: "ShoppingCartItems");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentTransactionId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Orders");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Shoes_ShoeId",
                table: "OrderItems",
                column: "ShoeId",
                principalTable: "Shoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
