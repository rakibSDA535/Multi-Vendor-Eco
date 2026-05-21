using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shop111.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddVendorIdAsNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VendorId",
                table: "Product",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Product_VendorId",
                table: "Product",
                column: "VendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_AspNetUsers_VendorId",
                table: "Product",
                column: "VendorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_AspNetUsers_VendorId",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_VendorId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "VendorId",
                table: "Product");
        }
    }
}
