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
            return _context.Users.ToList();
        }
    }
}
