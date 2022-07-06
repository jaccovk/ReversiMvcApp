using Microsoft.EntityFrameworkCore.Migrations;

namespace ReversiMvcApp.Data.Migrations
{
    public partial class changedSpeler : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Spelers",
                table: "Spelers");

            migrationBuilder.AlterColumn<string>(
                name: "Guid",
                table: "Spelers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Spelers",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Spelers",
                table: "Spelers",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Spelers",
                table: "Spelers");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Spelers");

            migrationBuilder.AlterColumn<string>(
                name: "Guid",
                table: "Spelers",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Spelers",
                table: "Spelers",
                column: "Guid");
        }
    }
}
