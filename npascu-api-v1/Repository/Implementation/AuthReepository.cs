using npascu_api_v1.Models.Entities.Auth;
using npascu_api_v1.Repository.Interface;
using System.Text;

namespace npascu_api_v1.Repository.Implementation
{
    public class AuthReepository : IAuthRepository
    {

        private readonly AppDbContext _context;

        public AuthReepository(AppDbContext context)
        {
            _context = context;
        }

        public RegisterModel RegisterUserAsync(string username, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                // Return an error or throw an exception for missing values
                throw new ArgumentException("Username, email, and password are required.");
            }

            if(IsEmailTaken(email))
            {
                throw new ArgumentException("Email is taken."); ;
            }

            var user = new ApplicationUser
            {
                Username = username,
                Email = email,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

             _context.ApplicationUsers.Add(user);
             _context.SaveChanges();

            return new RegisterModel()
            {
                Email = user.Email,
                UserName = user.Username,
                Password = password
            };
        }

        public LoginModel LoginUserAsync(string username, string password)
        {
            var user =  _context.ApplicationUsers.SingleOrDefault(x => x.Username == username);

            if (user == null)
                return null;

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return new LoginModel()
            {
                UserName = user.Username,
                Password = password
            };
        }

        public bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private bool IsEmailTaken(string email)
        {
            // Check if the email is already in use by querying the database
            return _context.ApplicationUsers.Any(u => u.Email == email);
        }
    }
}
