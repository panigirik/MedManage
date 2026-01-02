using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace MedManage.Persistence.Migrations
{
    public partial class InitialMedManageMigration : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Users",
            columns: table => new
            {
                Id = table.Column<Guid>(nullable: false, defaultValueSql: "gen_random_uuid()"),
                UserName = table.Column<string>(maxLength: 64, nullable: false),
                FullName = table.Column<string>(maxLength: 128, nullable: false),
                Role = table.Column<int>(nullable: false),
                CreatedAt = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                PhoneNumber = table.Column<string>(maxLength: 20, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Admins",
            columns: table => new
            {
                Id = table.Column<Guid>(nullable: false),
                CanManageUsers = table.Column<bool>(nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Admins", x => x.Id);
                table.ForeignKey(
                    name: "FK_Admins_Users_Id",
                    column: x => x.Id,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "SpecialUsers",
            columns: table => new
            {
                Id = table.Column<Guid>(nullable: false),
                SpecialRights = table.Column<string>(maxLength: 500, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SpecialUsers", x => x.Id);
                table.ForeignKey(
                    name: "FK_SpecialUsers_Users_Id",
                    column: x => x.Id,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Products",
            columns: table => new
            {
                Id = table.Column<Guid>(nullable: false, defaultValueSql: "gen_random_uuid()"),
                Name = table.Column<string>(maxLength: 255, nullable: false),
                Type = table.Column<int>(nullable: false),
                Quantity = table.Column<int>(nullable: false),
                Price = table.Column<decimal>(nullable: false),
                ExpirationDate = table.Column<DateTime>(nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Products", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Inventories",
            columns: table => new
            {
                ProductId = table.Column<Guid>(nullable: false),
                QuantityInStock = table.Column<int>(nullable: false),
                LastUpdated = table.Column<DateTime>(nullable: false),
                Status = table.Column<int>(nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Inventories", x => x.ProductId);
                table.ForeignKey(
                    name: "FK_Inventories_Products_ProductId",
                    column: x => x.ProductId,
                    principalTable: "Products",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Organizations",
            columns: table => new
            {
                Id = table.Column<Guid>(nullable: false, defaultValueSql: "gen_random_uuid()"),
                Name = table.Column<string>(maxLength: 255, nullable: false),
                Address = table.Column<string>(maxLength: 255, nullable: false),
                PhoneNumber = table.Column<string>(maxLength: 20, nullable: false),
                Email = table.Column<string>(maxLength: 128, nullable: false),
                CreatedAt = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Organizations", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Organizations");
        migrationBuilder.DropTable(name: "Inventories");
        migrationBuilder.DropTable(name: "Products");
        migrationBuilder.DropTable(name: "SpecialUsers");
        migrationBuilder.DropTable(name: "Admins");
        migrationBuilder.DropTable(name: "Users");
    }
}

}
