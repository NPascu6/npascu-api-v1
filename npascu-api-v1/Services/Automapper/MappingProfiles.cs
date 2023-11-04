using AutoMapper;
using npascu_api_v1.Models.DTOs;
using npascu_api_v1.Models.Entities;

namespace npascu_api_v1.Services.Automapper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Item, ItemDto>();
            CreateMap<ItemDto, Item>();


            CreateMap<Order, OrderDto>()
               .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
               .ReverseMap();

            CreateMap<OrderItem, OrderItemDto>()
               .ForMember(dest => dest.Order, opt => opt.MapFrom(src => src.Order))
               .ForMember(dest => dest.Item, opt => opt.MapFrom(src => src.Item))
               .ReverseMap();


            CreateMap<User, UserDto>()
              .ForMember(dest => dest.OwnedItems, opt => opt.MapFrom(src => src.OwnedItems))
              .ForMember(dest => dest.Orders, opt => opt.MapFrom(src => src.Orders))
              .ReverseMap();

            CreateMap<User, UserDto>()
              .ForMember(dest => dest.OwnedItems, opt => opt.MapFrom(src => src.OwnedItems))
              .ForMember(dest => dest.Orders, opt => opt.MapFrom(src => src.Orders))
              .ReverseMap();
        }
    }
}
