using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shop111.Data.Migrations
{
    /// <inheritdoc />
    public partial class VendorLocatiuonclass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartDetail_AspNetUsers_VendorId",
                table: "CartDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_AspNetUsers_VendorId",
                table: "OrderDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_AspNetUsers_VendorId",
                table: "Product");

            migrationBuilder.AddColumn<int>(
                name: "VendorId1",
                table: "Product",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Area = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vendor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VendorImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VendorAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VendorEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VendorNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VendorDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VendorType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TradeLicenceNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OwnerImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OwnerDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JoinDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsLive = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vendor_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Vendor_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Product_VendorId1",
                table: "Product",
                column: "VendorId1");

            migrationBuilder.CreateIndex(
                name: "IX_Vendor_LocationId",
                table: "Vendor",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Vendor_UserId",
                table: "Vendor",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CartDetail_AspNetUsers_VendorId",
                table: "CartDetail",
                column: "VendorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_AspNetUsers_VendorId",
                table: "OrderDetail",
                column: "VendorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartDetail_AspNetUsers_VendorId",
                table: "CartDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_AspNetUsers_VendorId",
                table: "OrderDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_AspNetUsers_VendorId",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_Vendor_VendorId1",
                table: "Product");

            migrationBuilder.DropTable(
                name: "Vendor");

            migrationBuilder.DropTable(
                name: "Location");

            migrationBuilder.DropIndex(
                name: "IX_Product_VendorId1",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "VendorId1",
                table: "Product");

            migrationBuilder.AddForeignKey(
                name: "FK_CartDetail_AspNetUsers_VendorId",
                table: "CartDetail",
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
                name: "FK_Product_AspNetUsers_VendorId",
                table: "Product",
                column: "VendorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
