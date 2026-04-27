using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccessManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ServerCreditsAndClientFinance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LimiteClientes",
                table: "servidores",
                newName: "QuantidadeCreditos");

            migrationBuilder.AddColumn<decimal>(
                name: "ValorCustoCredito",
                table: "servidores",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<Guid>(
                name: "TelaClienteId",
                table: "lancamentos_financeiros",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValorCustoCredito",
                table: "servidores");

            migrationBuilder.RenameColumn(
                name: "QuantidadeCreditos",
                table: "servidores",
                newName: "LimiteClientes");

            migrationBuilder.AlterColumn<Guid>(
                name: "TelaClienteId",
                table: "lancamentos_financeiros",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");
        }
    }
}
