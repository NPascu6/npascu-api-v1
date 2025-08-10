using System.Text.Json;
using Domain.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<DeductionRequestValidator>();

var app = builder.Build();

app.UseSerilogRequestLogging();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapPost("/v1/deductions/estimate", async (DeductionRequest req, IValidator<DeductionRequest> validator, AppDbContext db) =>
{
    var validation = await validator.ValidateAsync(req);
    if (!validation.IsValid)
        return Results.ValidationProblem(validation.ToDictionary());

    var rules = await db.CantonRuleSets.FirstOrDefaultAsync(r => r.Canton == req.Canton && r.Year == req.Year);
    if (rules == null) return Results.BadRequest(new { error = "Rules not found" });

    var rulesDoc = JsonDocument.Parse(rules.JsonRules);
    var deductions = rulesDoc.RootElement.GetProperty("deductions");

    decimal taxable = req.GrossIncome;
    if (deductions.TryGetProperty("pillar3a_max", out var p3))
        taxable -= Math.Min(req.Pillar3aContributions, p3.GetDecimal());

    var result = new DeductionEstimateResponse
    {
        EstimatedTaxableIncome = taxable
    };

    var federal = await db.FederalBrackets.FirstAsync(b => b.Year == req.Year);
    var fedBrackets = JsonSerializer.Deserialize<List<Bracket>>(federal.BracketsJson) ?? new();
    result.FederalTax = TaxCalculator.ComputeProgressiveTax(taxable, fedBrackets.Select(b => (b.up_to, b.rate)));

    var cantonal = await db.CantonalBrackets.FirstAsync(b => b.Canton == req.Canton && b.Year == req.Year);
    var cantBrackets = JsonSerializer.Deserialize<List<Bracket>>(cantonal.BracketsJson) ?? new();
    var cantTax = TaxCalculator.ComputeProgressiveTax(taxable, cantBrackets.Select(b => (b.up_to, b.rate)));
    result.CantonalTax = cantTax;

    var multiplier = await db.MunicipalityMultipliers.FirstAsync(m => m.Canton == req.Canton && m.Year == req.Year);
    result.CommunalTax = decimal.Round(cantTax * multiplier.Multiplier, 2);
    result.TotalTax = result.FederalTax + result.CantonalTax + result.CommunalTax;

    return Results.Ok(result);
});

app.MapGet("/v1/allowances/{canton}/{year:int}", async (string canton, int year, AppDbContext db) =>
{
    var rules = await db.CantonRuleSets.FirstOrDefaultAsync(r => r.Canton == canton && r.Year == year);
    if (rules == null) return Results.NotFound();
    var doc = JsonDocument.Parse(rules.JsonRules);
    var ded = doc.RootElement.GetProperty("deductions");
    return Results.Ok(JsonSerializer.Deserialize<object>(ded.GetRawText())!);
});

app.Run();

public record DeductionRequest(string Canton, int Year, decimal GrossIncome, decimal Pillar3aContributions);

public record DeductionEstimateResponse
{
    public decimal EstimatedTaxableIncome { get; set; }
    public decimal FederalTax { get; set; }
    public decimal CantonalTax { get; set; }
    public decimal CommunalTax { get; set; }
    public decimal TotalTax { get; set; }
}

public record Bracket(decimal up_to, decimal rate);

public class DeductionRequestValidator : AbstractValidator<DeductionRequest>
{
    public DeductionRequestValidator()
    {
        RuleFor(x => x.Canton).NotEmpty().Length(2);
        RuleFor(x => x.Year).GreaterThan(2000);
        RuleFor(x => x.GrossIncome).GreaterThan(0);
        RuleFor(x => x.Pillar3aContributions).GreaterThanOrEqualTo(0);
    }
}
