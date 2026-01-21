using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportStore.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderReference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrderReference",
                table: "Orders",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderReference",
                table: "Orders");
        }
    }
}
