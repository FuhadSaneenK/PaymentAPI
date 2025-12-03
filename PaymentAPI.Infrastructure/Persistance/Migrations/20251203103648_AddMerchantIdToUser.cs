using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMerchantIdToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MerchantId",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_MerchantId",
                table: "Users",
                column: "MerchantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Merchants_MerchantId",
                table: "Users",
                column: "MerchantId",
                principalTable: "Merchants",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Merchants_MerchantId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_MerchantId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MerchantId",
                table: "Users");
        }
    }
}
