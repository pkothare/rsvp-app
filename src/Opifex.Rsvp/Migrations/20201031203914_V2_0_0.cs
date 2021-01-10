using Microsoft.EntityFrameworkCore.Migrations;

namespace Opifex.Rsvp.Migrations
{
    public partial class V2_0_0 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Guests",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "Guests",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Extension",
                table: "Guests",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Guests");

            migrationBuilder.DropColumn(
                name: "Extension",
                table: "Guests");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Guests",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 250);
        }
    }
}
