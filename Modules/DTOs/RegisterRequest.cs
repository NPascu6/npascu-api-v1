namespace npascu_api_v1.Modules.DTOs
{
    public record RegisterRequest
    {
        public required string Name { get; init; }
        public required string Email { get; init; }
        public required string Password { get; init; }
    }
}