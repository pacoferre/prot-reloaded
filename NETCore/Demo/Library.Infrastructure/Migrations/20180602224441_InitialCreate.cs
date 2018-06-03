using Microsoft.EntityFrameworkCore.Migrations;

namespace Demo.Library.Infrastructure.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "appuser_hilo",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "AppUser",
                columns: table => new
                {
                    idAppUser = table.Column<int>(nullable: false),
                    name = table.Column<string>(maxLength: 30, nullable: false),
                    surname = table.Column<string>(maxLength: 50, nullable: false),
                    su = table.Column<bool>(nullable: false, defaultValue: false),
                    email = table.Column<string>(maxLength: 200, nullable: false),
                    password = table.Column<string>(maxLength: 400, nullable: false),
                    deactivated = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUser", x => x.idAppUser);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppUser");

            migrationBuilder.DropSequence(
                name: "appuser_hilo");
        }
    }
}
