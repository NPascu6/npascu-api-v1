using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace npascu_api_v1.Migrations
{
    /// <inheritdoc />
    public partial class ADD_VERIFICATION_TOKEN_APPLICATION_USER : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VerificationToken",
                table: "ApplicationUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VerificationToken",
                table: "ApplicationUsers");
        }
    }
}
