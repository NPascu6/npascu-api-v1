using AutoMapper;
using npascu_api_v1.Models.DTOs;
using npascu_api_v1.Repository.Interface;
using npascu_api_v1.Services.Interface;
using System;
using System.Collections.Generic;

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
            try
            {
                var items = _itemRepository.GetItems();

                if (items == null)
                {
                   return Enumerable.Empty<ItemDto>();
                }

                return _mapper.Map<IEnumerable<ItemDto>>(items);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
