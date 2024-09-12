using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Market.Migrations
{
    /// <inheritdoc />
    public partial class ChangeAddressPropertiesNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE `Addresses` CHANGE `Longitud` `Longitude` int;");
            migrationBuilder.Sql("ALTER TABLE `Addresses` CHANGE `Latitud` `Latitude` int;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Longitude",
                table: "Addresses",
                newName: "Longitud");

            migrationBuilder.RenameColumn(
                name: "Latitude",
                table: "Addresses",
                newName: "Latitud");

            migrationBuilder.AlterColumn<int>(
                name: "AddressId",
                table: "Purchase",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
