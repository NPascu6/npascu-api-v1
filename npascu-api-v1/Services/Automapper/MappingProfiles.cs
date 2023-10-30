using AutoMapper;
using npascu_api_v1.Models.DTOs;
using npascu_api_v1.Models.Entities;

namespace npascu_api_v1.Services.Automapper
{
    public class MappingProfiles: Profile
    {
        public MappingProfiles()
        {
            CreateMap<Item, ItemDto>();
            CreateMap<Order, OrderDto> ();
            CreateMap<OrderItem, OrderItemDto> ();
            CreateMap<User, UserDto>();
            CreateMap<UserItem, UserItemDto>();
        }
    }
}
