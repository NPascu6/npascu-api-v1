using npascu_api_v1.Models.Entities;
using npascu_api_v1.Repository.Interface;

namespace npascu_api_v1.Repository.Implementation
{
    public class UserRepository: IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<User> GetUsers()
        {
            try
            {
                var users = _context.Users.ToList();
                return users;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting users.", ex);
            }
        }

        public User CreateUser(User user)
        {
            try
            {
                _context.Users.Add(user);
                _context.SaveChanges();
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating a user.", ex);
            }
        }

        public User UpdateUser(int userId, User updatedUser)
        {
            try
            {
                var existingUser = _context.Users.Find(userId);

                if (existingUser == null)
                {
                    return null; // User with the specified ID not found
                }

                _context.Entry(existingUser).CurrentValues.SetValues(updatedUser);
                _context.SaveChanges();
                return existingUser;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating a user.", ex);
            }
        }

        public bool DeleteUser(int userId)
        {
            try
            {
                var userToDelete = _context.Users.Find(userId);

                if (userToDelete == null)
                {
                    return false; // User with the specified ID not found
                }

                _context.Users.Remove(userToDelete);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting a user.", ex);
            }
        }

        public User GetUserById(int userId)
        {
            try
            {
                var user = _context.Users.Find(userId);
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting a user by ID.", ex);
            }
        }
    }
}
