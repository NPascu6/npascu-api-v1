using AutoMapper;
using npascu_api_v1.Models.DTOs;
using npascu_api_v1.Models.Entities;
using npascu_api_v1.Repository.Interface;
using npascu_api_v1.Services.Interface;

namespace npascu_api_v1.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }
        public IEnumerable<UserDto> GetUsers()
        {
            try
            {
                var users = _userRepository.GetUsers();
                return _mapper.Map<IEnumerable<UserDto>>(users);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting users.", ex);
            }
        }

        public UserDto CreateUser(UserDto userDto)
        {
            try
            {
                var user = _mapper.Map<User>(userDto);
                // Perform any necessary validation or user creation logic here
                var createdUser = _userRepository.CreateUser(user);
                return _mapper.Map<UserDto>(createdUser);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating a user.", ex);
            }
        }

        public UserDto UpdateUser(int userId, UserDto userDto)
        {
            try
            {
                var updatedUser = _mapper.Map<User>(userDto);
                // Perform any necessary validation or user update logic here
                var existingUser = _userRepository.UpdateUser(userId, updatedUser);

                if (existingUser == null)
                {
                    return null; // User with the specified ID not found
                }

                return _mapper.Map<UserDto>(existingUser);
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
                return _userRepository.DeleteUser(userId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred whilte deliting user.", ex);
            };
        }

        public UserDto GetUserById(int userId)
        {
            try
            {
                var user = _userRepository.GetUserById(userId);

                if (user == null)
                {
                    return null; // User with the specified ID not found
                }

                return _mapper.Map<UserDto>(user);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting a user by ID.", ex);
            }
        }
    }
}
