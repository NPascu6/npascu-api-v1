using System.ComponentModel.DataAnnotations;

namespace npascu_api_v1.Models.Entities.Auth
{
    public class RegisterModel : LoginModel
    {
        [Required]
        public string? Email { get; set; }
    }
}
