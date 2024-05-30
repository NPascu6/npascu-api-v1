using npascu_api_v1.Models.DTOs.Auth;
using npascu_api_v1.Models.Entities.Auth;
using npascu_api_v1.Repository.Interface;
using System.Text;

namespace npascu_api_v1.Repository.Implementation
{
    public class AuthRepository : IAuthRepository
    {

        private readonly AppDbContext _context;

        public AuthRepository(AppDbContext context)
        {
            _context = context;
        }

        public ApplicationUser RegisterUserAsync(string username, string email, string password, string registrationToken)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Username, email, and password are required.");
            }

            if (IsEmailTaken(email))
            {
                throw new ArgumentException("Email is taken.");
            }

            if (IsUserTaken(username))
            {
                throw new ArgumentException("Username is taken.");
            }

            var user = new ApplicationUser
            {
                Username = username,
                Email = email,
                VerificationToken = registrationToken,
            };

            user.CreatedAt = DateTime.UtcNow;

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.ApplicationUsers.Add(user);
            _context.SaveChanges();

            return new ApplicationUser()
            {
                Id = user.Id,
                Email = user.Email
            };
        }

        public LoginModel LoginUserAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Username and password are required.");
            }
            var user = _context.ApplicationUsers.SingleOrDefault(x => x.Username == username);

            if (user == null || user.PasswordHash == null || user.PasswordSalt == null)
                throw new ArgumentException("User not found.");

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                throw new ArgumentException("Invalid password.");

            return new LoginModel()
            {
                UserName = user.Username,
                Password = password
            };
        }

        public bool ValidateEmail(string token)
        {
            var user = _context.ApplicationUsers.SingleOrDefault(u => u.VerificationToken == token);

            if (user != null)
            {
                user.IsVerified = true;
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DeleteUser(string email)
        {
            var user = _context.ApplicationUsers.SingleOrDefault(u => u.Email == email);
            if (user != null)
            {
                _context.ApplicationUsers.Remove(user);
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<string> GetUnvalidatedEmails()
        {
            List<ApplicationUser> users = _context.ApplicationUsers.Where(u => u.IsVerified == false).ToList();

            if (users != null && users.Count > 0)
            {
                List<string> unvalidatedEmails = new();

                foreach (var item in users)
                {
                    if (item != null && item.Email != null)
                    {
                        unvalidatedEmails.Add(item.Email);
                    }

                }
                if (unvalidatedEmails.Count > 0)
                {
                    return unvalidatedEmails;
                }
                else
                {
                    return new List<string>();
                }

            }
            else
            {
                return new List<string>();
            }
        }

        public ApplicationUser GetUser(string email)
        {
            return _context.ApplicationUsers.Single(u => u.Email == email);
        }

        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
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

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
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

        private bool IsUserTaken(string user)
        {
            return _context.ApplicationUsers.Any(u => u.Username == user);
        }
    }
}
