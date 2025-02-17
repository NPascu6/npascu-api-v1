namespace npascu_api_v1.Data.Models;

public record User
{
    public int Id { get; init; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
}