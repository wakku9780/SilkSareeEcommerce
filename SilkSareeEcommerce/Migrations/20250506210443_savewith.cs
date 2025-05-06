using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SilkSareeEcommerce.Migrations
{
    /// <inheritdoc />
    public partial class savewith : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_SavedAddresses_ShippingAddressId",
                table: "Orders");

            migrationBuilder.AlterColumn<int>(
                name: "ShippingAddressId",
                table: "Orders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_SavedAddresses_ShippingAddressId",
                table: "Orders",
                column: "ShippingAddressId",
                principalTable: "SavedAddresses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_SavedAddresses_ShippingAddressId",
                table: "Orders");

            migrationBuilder.AlterColumn<int>(
                name: "ShippingAddressId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_SavedAddresses_ShippingAddressId",
                table: "Orders",
                column: "ShippingAddressId",
                principalTable: "SavedAddresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
