using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SilkSareeEcommerce.Migrations
{
    /// <inheritdoc />
    public partial class addusernametoProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "ProductReviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "ProductReviews");
        }
    }
}
