using System.ComponentModel.DataAnnotations;

namespace npascu_api_v1.Data.Models;

public record UserCredentials
{
    [Key] public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Email { get; set; }
}