using AutoMapper;
using npascu_api_v1.Models.DTOs.Item;
using npascu_api_v1.Models.DTOs.Order;
using npascu_api_v1.Models.DTOs.User;
using npascu_api_v1.Models.Entities;

namespace npascu_api_v1.Services.Automapper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Item, ItemDto>().ReverseMap();

            CreateMap<CreateOrderDto, Order>();
            CreateMap<Order, OrderDto>().ReverseMap();
            CreateMap<CreateOrderItemDto, OrderItem>();
            CreateMap<OrderItem, OrderItemDto>().ReverseMap();

            CreateMap<CreateUserDto, User>();
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<UserItem, UserItemDto>().ReverseMap();
        }
    }
}
