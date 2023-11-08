using System.ComponentModel.DataAnnotations;

namespace npascu_api_v1.Models.DTOs.Auth
{
    public class RegisterModel : LoginModel
    {
        [Required]
        public string? Email { get; set; }
    }
}
