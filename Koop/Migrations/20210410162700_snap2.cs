using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Koop.Migrations
{
    public partial class snap2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoopName",
                table: "baskets");

            migrationBuilder.RenameColumn(
                name: "CoopId",
                table: "baskets",
                newName: "coop_id");

            migrationBuilder.AddColumn<bool>(
                name: "available",
                table: "suppliers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "blocked",
                table: "suppliers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "coop_id",
                table: "baskets",
                type: "uuid",
                nullable: true,
                defaultValueSql: "uuid_generate_v4()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "baskets_view",
                columns: table => new
                {
                    basket_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    cooperator = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "order_grande_history_view",
                columns: table => new
                {
                    order_id = table.Column<Guid>(type: "uuid", nullable: true),
                    order_start_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    order_stop_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    order_status_id = table.Column<Guid>(type: "uuid", nullable: true),
                    order_status_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "supplier_view",
                columns: table => new
                {
                    supplier_id = table.Column<Guid>(type: "uuid", nullable: false),
                    blocked = table.Column<bool>(type: "boolean", nullable: false),
                    available = table.Column<bool>(type: "boolean", nullable: false),
                    supplier_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    supplier_abbr = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    email = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    picture = table.Column<string>(type: "text", nullable: true),
                    order_closing_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    opro_id = table.Column<Guid>(type: "uuid", nullable: false),
                    opro_first_name = table.Column<string>(type: "text", nullable: true),
                    opro_last_name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "baskets_view");

            migrationBuilder.DropTable(
                name: "order_grande_history_view");

            migrationBuilder.DropTable(
                name: "supplier_view");

            migrationBuilder.DropColumn(
                name: "available",
                table: "suppliers");

            migrationBuilder.DropColumn(
                name: "blocked",
                table: "suppliers");

            migrationBuilder.RenameColumn(
                name: "coop_id",
                table: "baskets",
                newName: "CoopId");

            migrationBuilder.AlterColumn<Guid>(
                name: "CoopId",
                table: "baskets",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true,
                oldDefaultValueSql: "uuid_generate_v4()");

            migrationBuilder.AddColumn<string>(
                name: "CoopName",
                table: "baskets",
                type: "text",
                nullable: true);
        }
    }
}
