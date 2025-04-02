using npascu_api_v1.Data.Models;

namespace npascu_api_v1.Modules.Services.Token
{
    public interface ITokenService
    {
        string GenerateToken(User user, TimeSpan? expiration = null);
        string AuthenticateAndGenerateToken(User user, string password, TimeSpan? expiration = null);
    }
}