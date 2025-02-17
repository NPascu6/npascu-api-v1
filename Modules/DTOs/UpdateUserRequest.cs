namespace npascu_api_v1.Modules.DTOs
{
    public record UpdateUserRequest
    {
        public required string Name { get; init; }
        public required string Email { get; init; }

        /// <summary>
        /// Optional – only used if updating a local user's password.
        /// </summary>
        public string? Password { get; init; }
    }
}