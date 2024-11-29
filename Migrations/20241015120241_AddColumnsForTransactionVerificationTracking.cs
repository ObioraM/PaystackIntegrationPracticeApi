using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaystackIntegrationPracticeApi.Migrations
{
    public partial class AddColumnsForTransactionVerificationTracking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TransactionVerificationMessage",
                table: "PaystackIntegration",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TransactionVerified",
                table: "PaystackIntegration",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerifiedTransactionStatus",
                table: "PaystackIntegration",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactionVerificationMessage",
                table: "PaystackIntegration");

            migrationBuilder.DropColumn(
                name: "TransactionVerified",
                table: "PaystackIntegration");

            migrationBuilder.DropColumn(
                name: "VerifiedTransactionStatus",
                table: "PaystackIntegration");
        }
    }
}
