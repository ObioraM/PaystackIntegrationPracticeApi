using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaystackIntegrationPracticeApi.Migrations
{
    public partial class IncludeAmountInBaseDenominationColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AmountInBaseDenomination",
                table: "PaystackIntegration",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountInBaseDenomination",
                table: "PaystackIntegration");
        }
    }
}
