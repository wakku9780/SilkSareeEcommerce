using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SilkSareeEcommerce.Migrations
{
    /// <inheritdoc />
    public partial class ReviewYaar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVerifiedBuyer",
                table: "ProductReviews");

            migrationBuilder.DropColumn(
                name: "ReviewText",
                table: "ProductReviews");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "ProductReviews");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "ProductReviews",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<decimal>(
                name: "AverageRating",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageRating",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "ProductReviews",
                newName: "CreatedDate");

            migrationBuilder.AddColumn<bool>(
                name: "IsVerifiedBuyer",
                table: "ProductReviews",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ReviewText",
                table: "ProductReviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "ProductReviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
