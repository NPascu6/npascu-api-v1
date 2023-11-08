using System.ComponentModel.DataAnnotations;

namespace npascu_api_v1.Models.DTOs.Auth
{
    public class LoginModel
    {
        [Required]
        public string? UserName { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
