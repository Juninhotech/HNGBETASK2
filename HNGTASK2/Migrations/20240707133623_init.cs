using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HNGTASK2.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "organisations",
                columns: table => new
                {
                    orgid = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_organisations", x => x.orgid);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    userid = table.Column<string>(type: "text", nullable: false),
                    firstname = table.Column<string>(type: "text", nullable: false),
                    lastname = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    password = table.Column<string>(type: "text", nullable: false),
                    phone = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.userid);
                });

            migrationBuilder.CreateTable(
                name: "userorganisations",
                columns: table => new
                {
                    userid = table.Column<string>(type: "text", nullable: false),
                    orgid = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userorganisations", x => new { x.userid, x.orgid });
                    table.ForeignKey(
                        name: "FK_userorganisations_organisations_orgid",
                        column: x => x.orgid,
                        principalTable: "organisations",
                        principalColumn: "orgid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_userorganisations_users_userid",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "userid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_organisations_orgid",
                table: "organisations",
                column: "orgid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_userorganisations_orgid",
                table: "userorganisations",
                column: "orgid");

            migrationBuilder.CreateIndex(
                name: "IX_users_userid_email",
                table: "users",
                columns: new[] { "userid", "email" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "userorganisations");

            migrationBuilder.DropTable(
                name: "organisations");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
