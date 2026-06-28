using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WynnwaveInvoice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoiceLinePeriod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "period_end",
                table: "invoice_line",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "period_start",
                table: "invoice_line",
                type: "date",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "period_end",
                table: "invoice_line");

            migrationBuilder.DropColumn(
                name: "period_start",
                table: "invoice_line");
        }
    }
}
