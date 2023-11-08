using npascu_api_v1.Models.DTOs.Auth;
using npascu_api_v1.Models.Entities.Auth;

namespace npascu_api_v1.Repository.Interface
{
    public interface IAuthRepository
    {
        ApplicationUser RegisterUserAsync(string username, string email, string password, string registrationToken);
        LoginModel LoginUserAsync(string username, string password);
        bool ValidateEmail(string token);
        ApplicationUser GetUser(string email);
        bool DeleteUser(string email);
        IEnumerable<string>? GetUnvalidatedEmails();
    }
}
