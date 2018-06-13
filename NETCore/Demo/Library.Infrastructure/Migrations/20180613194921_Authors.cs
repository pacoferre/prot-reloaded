using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Demo.Library.Infrastructure.Migrations
{
    public partial class Authors : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuthorNationality",
                columns: table => new
                {
                    idAuthorNationality = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 70, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorNationality", x => x.idAuthorNationality);
                });

            migrationBuilder.CreateTable(
                name: "Author",
                columns: table => new
                {
                    idAuthor = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    idAuthorNationality = table.Column<int>(maxLength: 50, nullable: false),
                    name = table.Column<string>(maxLength: 30, nullable: false),
                    surname = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Author", x => x.idAuthor);
                    table.ForeignKey(
                        name: "FK_Author_AuthorNationality_idAuthorNationality",
                        column: x => x.idAuthorNationality,
                        principalTable: "AuthorNationality",
                        principalColumn: "idAuthorNationality",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Author_idAuthorNationality",
                table: "Author",
                column: "idAuthorNationality");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Author");

            migrationBuilder.DropTable(
                name: "AuthorNationality");
        }
    }
}
