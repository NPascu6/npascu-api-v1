namespace npascu_api_v1.Modules.DTOs
{
    public record LoginRequest
    {
        public required string Email { get; init; }
        public required string Password { get; init; }
    }
}