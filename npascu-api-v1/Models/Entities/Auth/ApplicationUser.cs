using System.ComponentModel.DataAnnotations;

namespace npascu_api_v1.Models.Entities.Auth
{
    public class ApplicationUser: ModelBase
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public byte[]? PasswordHash { get; set; }
        [Required]
        public byte[]? PasswordSalt { get; set; }

        public bool IsVerified { get; set; }
    }
}
