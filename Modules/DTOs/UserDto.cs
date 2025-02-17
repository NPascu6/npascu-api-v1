namespace npascu_api_v1.Modules.DTOs
{
    public record UserDto
    {
        public int Id { get; init; }
        public required string Name { get; init; }
        public required string Email { get; init; }
        public required string AuthProvider { get; init; }
        public List<string>? UserRoles { get; init; }
    }
}