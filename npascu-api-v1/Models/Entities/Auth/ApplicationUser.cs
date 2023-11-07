using System.ComponentModel.DataAnnotations;

namespace npascu_api_v1.Models.Entities.Auth
{
    public class ApplicationUser: ModelBase
    {
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}
