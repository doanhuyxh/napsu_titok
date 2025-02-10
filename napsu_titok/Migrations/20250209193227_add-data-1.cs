using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace napsu_titok.Migrations
{
    /// <inheritdoc />
    public partial class adddata1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Amount",
                table: "DataUser",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "DataUser",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "DataUser");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "DataUser");
        }
    }
}
