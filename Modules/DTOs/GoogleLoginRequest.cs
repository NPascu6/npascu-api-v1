namespace npascu_api_v1.Modules.DTOs
{
    public record GoogleLoginRequest
    {
        /// <summary>
        /// The token obtained from Google (to be validated).
        /// </summary>
        public required string Token { get; init; }
    }
}