namespace npascu_api_v1.Services.Interface
{
    public interface IAuthService
    {
        string Login(string username, string password);
        string Register(string username, string email, string password);
        bool ValidateEmail(string token);
        bool DeleteUser(string username);
    }
}
