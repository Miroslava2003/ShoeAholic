using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoeAholic.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderStatusAndAdminNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminNotes",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminNotes",
                table: "Orders");
        }
    }
}
