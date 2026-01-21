using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportStore.Migrations
{
    /// <inheritdoc />
    public partial class updateOrderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Line2",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Line3",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "Line1",
                table: "Orders",
                newName: "Address");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Orders",
                newName: "Line1");

            migrationBuilder.AddColumn<string>(
                name: "Line2",
                table: "Orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Line3",
                table: "Orders",
                type: "text",
                nullable: true);
        }
    }
}
