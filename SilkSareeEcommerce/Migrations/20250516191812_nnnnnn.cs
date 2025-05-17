using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SilkSareeEcommerce.Migrations
{
    /// <inheritdoc />
    public partial class nnnnnn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "ProductReviews",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "ProductReviews");
        }
    }
}
