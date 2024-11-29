using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaystackIntegrationPracticeApi.Migrations
{
    public partial class AddIntializeTransactionResponseMessageColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IntializeTransactionResponseMessage",
                table: "PaystackIntegration",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IntializeTransactionResponseMessage",
                table: "PaystackIntegration");
        }
    }
}
