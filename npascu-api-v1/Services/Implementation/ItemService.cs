using AutoMapper;
using npascu_api_v1.Models.DTOs;
using npascu_api_v1.Repository.Interface;
using npascu_api_v1.Services.Interface;

namespace npascu_api_v1.Services.Implementation
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _itemRepository;
        private readonly IMapper _mapper;

        public ItemService(IItemRepository itemRepository, IMapper mapper)
        {
            _itemRepository = itemRepository;
            _mapper = mapper;
        }
        public IEnumerable<ItemDto> GetItems()
        {
            var items = _itemRepository.GetItems();

            return _mapper.Map<IEnumerable<ItemDto>>(items);
        }
    }
}
