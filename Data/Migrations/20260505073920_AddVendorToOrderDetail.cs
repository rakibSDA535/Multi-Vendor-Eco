using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shop111.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddVendorToOrderDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VendorId",
                table: "Stock",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VendorId",
                table: "ShoppingCart",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VendorId",
                table: "OrderDetail",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VendorId",
                table: "Order",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VendorId",
                table: "CartDetail",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stock_VendorId",
                table: "Stock",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCart_VendorId",
                table: "ShoppingCart",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_VendorId",
                table: "OrderDetail",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_VendorId",
                table: "Order",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_CartDetail_VendorId",
                table: "CartDetail",
                column: "VendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartDetail_AspNetUsers_VendorId",
                table: "CartDetail",
                column: "VendorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_AspNetUsers_VendorId",
                table: "Order",
                column: "VendorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_AspNetUsers_VendorId",
                table: "OrderDetail",
                column: "VendorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCart_AspNetUsers_VendorId",
                table: "ShoppingCart",
                column: "VendorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Stock_AspNetUsers_VendorId",
                table: "Stock",
                column: "VendorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartDetail_AspNetUsers_VendorId",
                table: "CartDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_AspNetUsers_VendorId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_AspNetUsers_VendorId",
                table: "OrderDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCart_AspNetUsers_VendorId",
                table: "ShoppingCart");

            migrationBuilder.DropForeignKey(
                name: "FK_Stock_AspNetUsers_VendorId",
                table: "Stock");

            migrationBuilder.DropIndex(
                name: "IX_Stock_VendorId",
                table: "Stock");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingCart_VendorId",
                table: "ShoppingCart");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetail_VendorId",
                table: "OrderDetail");

            migrationBuilder.DropIndex(
                name: "IX_Order_VendorId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_CartDetail_VendorId",
                table: "CartDetail");

            migrationBuilder.DropColumn(
                name: "VendorId",
                table: "Stock");

            migrationBuilder.DropColumn(
                name: "VendorId",
                table: "ShoppingCart");

            migrationBuilder.DropColumn(
                name: "VendorId",
                table: "OrderDetail");

            migrationBuilder.DropColumn(
                name: "VendorId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "VendorId",
                table: "CartDetail");
        }
    }
}
