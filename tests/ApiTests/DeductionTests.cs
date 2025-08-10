using System.Linq;
using System.Net;
using System.Net.Http.Json;
using Api;
using Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Api.Background;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace ApiTests;

public class DeductionTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public DeductionTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DbContextOptions<AppDbContext>>();
                services.RemoveAll<AppDbContext>();
                services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("tests"));

                var hosted = services.SingleOrDefault(d => d.ImplementationType == typeof(FinnhubRestService));
                if (hosted != null) services.Remove(hosted);
            });
        });
    }

    [Fact]
    public async Task Health_ReturnsOk()
    {
        var client = _factory.CreateClient();
        var resp = await client.GetAsync("/health");
        Assert.True(resp.IsSuccessStatusCode);
    }

}
