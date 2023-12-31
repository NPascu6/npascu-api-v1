﻿using npascu_api_v1.Models.DTOs.User;

namespace npascu_api_v1.Services.Interface
{
    public interface IUserService
    {
        IEnumerable<UserDto> GetUsers();
        UserDto CreateUser(CreateUserDto userDto);
        UserDto UpdateUser(int userId, UserDto userDto);
        bool DeleteUser(int userId);
        UserDto GetUserById(int userId);
    }
}
