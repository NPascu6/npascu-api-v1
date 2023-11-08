using npascu_api_v1.Models.Entities;

namespace npascu_api_v1.Repository.Interface
{
    public interface IUserRepository
    {
        IEnumerable<User> GetUsers();
        User CreateUser(User user);
        User UpdateUser(int userId, User updatedUser);
        bool DeleteUser(int userId);
        User GetUserById(int userId);
    }
}
