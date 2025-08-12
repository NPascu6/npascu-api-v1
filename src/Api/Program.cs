using System.Text.Json;
using System.IO;
using System.Reflection;
using Domain.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure;
using Infrastructure.Clients;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Api.Background;
using Api.Hubs;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Any;
using Swashbuckle.AspNetCore.Filters;
using Api.SwaggerExamples;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "SwissTax API", Version = "v1" });
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    options.EnableAnnotations();
    options.ExampleFilters();
});
builder.Services.AddSwaggerExamplesFromAssemblyOf<QuoteExample>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<DeductionRequestValidator>();
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddHttpClient<FinnhubRestService>();
builder.Services.AddHostedService<FinnhubRestService>();
builder.Services.Configure<FinnhubOptions>(builder.Configuration.GetSection("Finnhub"));
builder.Services.AddHttpClient<IFinnhubClient, FinnhubClient>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("https://pascu.io")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "SwissTax API");
});
app.UseCors();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }))
   .WithName("GetHealth")
   .WithTags("Health")
   .WithGroupName("v1")
   .WithOpenApi(op =>
   {
       op.Summary = "Health check";
       op.Description = "Returns API availability status.";
       op.Responses["200"] = new OpenApiResponse
       {
           Description = "OK",
           Content =
           {
               ["application/json"] = new OpenApiMediaType
               {
                   Example = new OpenApiObject
                   {
                       ["status"] = new OpenApiString("ok")
                   }
               }
           }
       };
       return op;
   });

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

    var federal = await db.FederalBrackets.FirstOrDefaultAsync(b => b.Year == req.Year);
    if (federal == null) return Results.BadRequest(new { error = "Federal brackets not found" });
    var fedBrackets = JsonSerializer.Deserialize<List<Bracket>>(federal.BracketsJson) ?? new();
    result.FederalTax = TaxCalculator.ComputeProgressiveTax(taxable, fedBrackets.Select(b => (b.up_to, b.rate)));

    var cantonal = await db.CantonalBrackets.FirstOrDefaultAsync(b => b.Canton == req.Canton && b.Year == req.Year);
    if (cantonal == null) return Results.BadRequest(new { error = "Cantonal brackets not found" });
    var cantBrackets = JsonSerializer.Deserialize<List<Bracket>>(cantonal.BracketsJson) ?? new();
    var cantTax = TaxCalculator.ComputeProgressiveTax(taxable, cantBrackets.Select(b => (b.up_to, b.rate)));
    result.CantonalTax = cantTax;

    var multiplier = await db.MunicipalityMultipliers.FirstOrDefaultAsync(m => m.Canton == req.Canton && m.Year == req.Year);
    if (multiplier == null) return Results.BadRequest(new { error = "Municipality multiplier not found" });
    result.CommunalTax = decimal.Round(cantTax * multiplier.Multiplier, 2);
    result.TotalTax = result.FederalTax + result.CantonalTax + result.CommunalTax;

    return Results.Ok(result);
})
   .WithName("EstimateDeductions")
    .WithTags("Deductions")
    .WithGroupName("v1")
    .WithOpenApi(op =>
    {
        op.Summary = "Estimate tax deductions";
        op.RequestBody = new OpenApiRequestBody
        {
            Content =
            {
                ["application/json"] = new OpenApiMediaType
                {
                    Example = new OpenApiObject
                    {
                        ["canton"] = new OpenApiString("ZH"),
                        ["year"] = new OpenApiInteger(2024),
                        ["grossIncome"] = new OpenApiDouble(100000),
                        ["pillar3aContributions"] = new OpenApiDouble(6883)
                    }
                }
            }
        };
        op.Responses["200"] = new OpenApiResponse
        {
            Description = "Tax estimate",
            Content =
            {
                ["application/json"] = new OpenApiMediaType
                {
                    Example = new OpenApiObject
                    {
                        ["estimatedTaxableIncome"] = new OpenApiDouble(93117),
                        ["federalTax"] = new OpenApiDouble(5000),
                        ["cantonalTax"] = new OpenApiDouble(3000),
                        ["communalTax"] = new OpenApiDouble(2000),
                        ["totalTax"] = new OpenApiDouble(10000)
                    }
                }
            }
        };
        return op;
    });

app.MapGet("/v1/allowances/{canton}/{year:int}", async (string canton, int year, AppDbContext db) =>
{
    var rules = await db.CantonRuleSets.FirstOrDefaultAsync(r => r.Canton == canton && r.Year == year);
    if (rules == null) return Results.NotFound();
    var doc = JsonDocument.Parse(rules.JsonRules);
    var ded = doc.RootElement.GetProperty("deductions");
    return Results.Ok(JsonSerializer.Deserialize<object>(ded.GetRawText())!);
})
   .WithName("GetAllowances")
   .WithTags("Allowances")
   .WithGroupName("v1")
   .WithOpenApi(op =>
   {
       op.Summary = "Get deduction allowances";
       op.Responses["200"] = new OpenApiResponse
       {
            Description = "Allowances",
            Content =
            {
                ["application/json"] = new OpenApiMediaType
                {
                    Example = new OpenApiObject
                    {
                        ["pillar3a_max"] = new OpenApiDouble(6883)
                    }
                }
            }
       };
       return op;
   });

app.MapHub<QuotesHub>("/quotesHub");
app.MapHub<MarketHub>("/hubs/market");
app.MapControllers();

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
