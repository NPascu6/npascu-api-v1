using AutoMapper;
using npascu_api_v1.Models.DTOs;
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
            var users = _userRepository.GetUsers();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }
    }
}
