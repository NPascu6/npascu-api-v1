namespace npascu_api_v1.Data.Models;

public record UserCredentials
{
    public string Username { get; set; }
    public string Password { get; set; }
}