using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CantonalBrackets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Canton = table.Column<string>(type: "text", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    BracketsJson = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CantonalBrackets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CantonRuleSets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Canton = table.Column<string>(type: "text", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    JsonRules = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CantonRuleSets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FederalBrackets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    BracketsJson = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FederalBrackets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MunicipalityMultipliers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Canton = table.Column<string>(type: "text", nullable: false),
                    MunicipalityCode = table.Column<string>(type: "text", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Multiplier = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MunicipalityMultipliers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Plan = table.Column<string>(type: "text", nullable: false),
                    StripeCustomerId = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApiKeys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Hash = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiKeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiKeys_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CantonRuleSets",
                columns: new[] { "Id", "Canton", "JsonRules", "Year" },
                values: new object[] { 1, "ZH", "{\"canton\":\"ZH\",\"year\":2025,\"deductions\":{\"pillar3a_max\":7400,\"commute\":{\"max\":3000,\"per_km\":0.7,\"method\":\"car|public\"},\"lunch_allowance_per_day\":15,\"home_office_max\":1500,\"childcare_max\":10000},\"quellensteuer\":{\"tables\":[]},\"cantonal_multipliers_note\":\"Use MunicipalityMultiplier table to compute communal tax.\"}", 2025 });

            migrationBuilder.InsertData(
                table: "CantonalBrackets",
                columns: new[] { "Id", "BracketsJson", "Canton", "Year" },
                values: new object[] { 1, "[{\"up_to\":30000,\"rate\":0.01},{\"up_to\":60000,\"rate\":0.025},{\"up_to\":100000,\"rate\":0.04}]", "ZH", 2025 });

            migrationBuilder.InsertData(
                table: "FederalBrackets",
                columns: new[] { "Id", "BracketsJson", "Year" },
                values: new object[] { 1, "[{\"up_to\":30000,\"rate\":0.02},{\"up_to\":60000,\"rate\":0.05},{\"up_to\":100000,\"rate\":0.08}]", 2025 });

            migrationBuilder.InsertData(
                table: "MunicipalityMultipliers",
                columns: new[] { "Id", "Canton", "Multiplier", "MunicipalityCode", "Year" },
                values: new object[] { 1, "ZH", 1.19m, "261", 2025 });

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_TenantId",
                table: "ApiKeys",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiKeys");

            migrationBuilder.DropTable(
                name: "CantonalBrackets");

            migrationBuilder.DropTable(
                name: "CantonRuleSets");

            migrationBuilder.DropTable(
                name: "FederalBrackets");

            migrationBuilder.DropTable(
                name: "MunicipalityMultipliers");

            migrationBuilder.DropTable(
                name: "Tenants");
        }
    }
}
