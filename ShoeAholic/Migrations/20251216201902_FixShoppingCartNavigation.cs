using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoeAholic.Migrations
{
    /// <inheritdoc />
    public partial class FixShoppingCartNavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCartItems_ShoeSizes_ShoeSizeId",
                table: "ShoppingCartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCartItems_Shoes_ShoeId1",
                table: "ShoppingCartItems");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingCartItems_ShoeId1",
                table: "ShoppingCartItems");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingCartItems_ShoeSizeId",
                table: "ShoppingCartItems");

            migrationBuilder.DropColumn(
                name: "ShoeId1",
                table: "ShoppingCartItems");

            migrationBuilder.DropColumn(
                name: "ShoeSizeId",
                table: "ShoppingCartItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCartItems_ShoeId1",
                table: "ShoppingCartItems",
                column: "ShoeId1");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCartItems_ShoeSizeId",
                table: "ShoppingCartItems",
                column: "ShoeSizeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCartItems_ShoeSizes_ShoeSizeId",
                table: "ShoppingCartItems",
                column: "ShoeSizeId",
                principalTable: "ShoeSizes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCartItems_Shoes_ShoeId1",
                table: "ShoppingCartItems",
                column: "ShoeId1",
                principalTable: "Shoes",
                principalColumn: "Id");
        }
    }
}
