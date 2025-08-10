using Infrastructure;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHostedService<PdfWorker>();

builder.Services.AddSerilog((ctx, lc) => lc.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration));

var app = builder.Build();
app.Run();

class PdfWorker : BackgroundService
{
    private readonly ILogger<PdfWorker> _logger;
    public PdfWorker(ILogger<PdfWorker> logger) => _logger = logger;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("PDF worker running");
        return Task.CompletedTask;
    }
}
