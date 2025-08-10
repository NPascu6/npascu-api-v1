using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<ApiKey> ApiKeys => Set<ApiKey>();
    public DbSet<CantonRuleSet> CantonRuleSets => Set<CantonRuleSet>();
    public DbSet<TaxBracketFederal> FederalBrackets => Set<TaxBracketFederal>();
    public DbSet<TaxBracketCantonal> CantonalBrackets => Set<TaxBracketCantonal>();
    public DbSet<MunicipalityMultiplier> MunicipalityMultipliers => Set<MunicipalityMultiplier>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Tenant>().HasMany(t => t.ApiKeys).WithOne(k => k.Tenant!).HasForeignKey(k => k.TenantId);

        // seed sample tax data
        modelBuilder.Entity<TaxBracketFederal>().HasData(new TaxBracketFederal
        {
            Id = 1,
            Year = 2025,
            BracketsJson = JsonSerializer.Serialize(new[]
            {
                new { up_to = 30000m, rate = 0.02m },
                new { up_to = 60000m, rate = 0.05m },
                new { up_to = 100000m, rate = 0.08m }
            })
        });

        modelBuilder.Entity<TaxBracketCantonal>().HasData(new TaxBracketCantonal
        {
            Id = 1,
            Canton = "ZH",
            Year = 2025,
            BracketsJson = JsonSerializer.Serialize(new[]
            {
                new { up_to = 30000m, rate = 0.01m },
                new { up_to = 60000m, rate = 0.025m },
                new { up_to = 100000m, rate = 0.04m }
            })
        });

        modelBuilder.Entity<MunicipalityMultiplier>().HasData(new MunicipalityMultiplier
        {
            Id = 1,
            Canton = "ZH",
            MunicipalityCode = "261",
            Year = 2025,
            Multiplier = 1.19m
        });

        modelBuilder.Entity<CantonRuleSet>().HasData(new CantonRuleSet
        {
            Id = 1,
            Canton = "ZH",
            Year = 2025,
            JsonRules = JsonSerializer.Serialize(new { canton = "ZH", year = 2025, deductions = new { pillar3a_max = 7400m, commute = new { max = 3000m, per_km = 0.7m, method = "car|public" }, lunch_allowance_per_day = 15m, home_office_max = 1500m, childcare_max = 10000m }, quellensteuer = new { tables = new object[0] }, cantonal_multipliers_note = "Use MunicipalityMultiplier table to compute communal tax." })
        });
    }
}
