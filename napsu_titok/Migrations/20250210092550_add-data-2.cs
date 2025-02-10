using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace napsu_titok.Migrations
{
    /// <inheritdoc />
    public partial class adddata2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "DataUser");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "DataUser",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
