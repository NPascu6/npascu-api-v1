using npascu_api_v1.Models.Entities.Auth;

namespace npascu_api_v1.Repository.Interface
{
    public interface IAuthRepository
    {
        RegisterModel RegisterUserAsync(string username, string email, string password);
        LoginModel LoginUserAsync(string username, string password);
    }
}
