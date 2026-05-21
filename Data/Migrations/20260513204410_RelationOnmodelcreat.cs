using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shop111.Data.Migrations
{
    /// <inheritdoc />
    public partial class RelationOnmodelcreat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_AspNetUsers_VendorId",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_Vendor_VendorId1",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_VendorId1",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "VendorId1",
                table: "Product");

            migrationBuilder.AlterColumn<int>(
                name: "VendorId",
                table: "Product",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Vendor_VendorId",
                table: "Product",
                column: "VendorId",
                principalTable: "Vendor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_Vendor_VendorId",
                table: "Product");

            migrationBuilder.AlterColumn<string>(
                name: "VendorId",
                table: "Product",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "VendorId1",
                table: "Product",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Product_VendorId1",
                table: "Product",
                column: "VendorId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_AspNetUsers_VendorId",
                table: "Product",
                column: "VendorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Vendor_VendorId1",
                table: "Product",
                column: "VendorId1",
                principalTable: "Vendor",
                principalColumn: "Id");
        }
    }
}
