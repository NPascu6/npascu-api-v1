namespace npascu_api_v1.Data.Models;

public record UserCredentials
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}