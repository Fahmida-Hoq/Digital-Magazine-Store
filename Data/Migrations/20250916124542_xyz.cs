using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalMagazineStore.Data.Migrations
{
    /// <inheritdoc />
    public partial class xyz : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Unitprice",
                table: "OrderDetail",
                newName: "UnitPrice");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Order",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Order",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "Order",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MobileNumber",
                table: "Order",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Order",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "Order",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "UnitPrice",
                table: "CartDetail",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "MobileNumber",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "CartDetail");

            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "OrderDetail",
                newName: "Unitprice");
        }
    }
}
