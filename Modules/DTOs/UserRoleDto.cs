namespace npascu_api_v1.Modules.DTOs
{
    public record UserRoleDto
    {
        public required int UserId { get; init; }
        public required string RoleName { get; init; }
    }
}