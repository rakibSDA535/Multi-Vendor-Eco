using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shop111.Data.Migrations
{
    /// <inheritdoc />
    public partial class VendortoAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Vendor_VendorId1",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_VendorId1",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "VendorId1",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "VendorId",
                table: "AspNetUsers",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_VendorId",
                table: "AspNetUsers",
                column: "VendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Vendor_VendorId",
                table: "AspNetUsers",
                column: "VendorId",
                principalTable: "Vendor",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Vendor_VendorId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_VendorId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "VendorId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VendorId1",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_VendorId1",
                table: "AspNetUsers",
                column: "VendorId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Vendor_VendorId1",
                table: "AspNetUsers",
                column: "VendorId1",
                principalTable: "Vendor",
                principalColumn: "Id");
        }
    }
}
