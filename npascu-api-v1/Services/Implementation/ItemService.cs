using AutoMapper;
using npascu_api_v1.Models.DTOs;
using npascu_api_v1.Models.Entities;
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

        public ItemDto CreateItem(ItemDto itemDto)
        {
            try
            {
                // Map the DTO to the entity
                var item = _mapper.Map<Item>(itemDto);

                // Perform any necessary validation or business logic here

                // Save the item in the repository
                var createdItem = _itemRepository.CreateItem(item);

                return _mapper.Map<ItemDto>(createdItem);
            }
            catch (Exception ex)
            {
                // You can log the exception and handle it as needed
                throw new Exception("An error occurred while creating an item.", ex);
            }
        }

        public ItemDto UpdateItem(int itemId, ItemDto itemDto)
        {
            try
            {
                // Map the DTO to the entity
                var item = _mapper.Map<Item>(itemDto);

                // Perform any necessary validation or business logic here

                // Update the item in the repository
                var updatedItem = _itemRepository.UpdateItem(itemId, item);

                if (updatedItem == null)
                {
                    return null; // Item with the specified ID not found
                }

                return _mapper.Map<ItemDto>(updatedItem);
            }
            catch (Exception ex)
            {
                // You can log the exception and handle it as needed
                throw new Exception("An error occurred while updating an item.", ex);
            }
        }

        public bool DeleteItem(int itemId)
        {
            try
            {
                var isDeleted = _itemRepository.DeleteItem(itemId);

                return isDeleted;
            }
            catch (Exception ex)
            {
                // You can log the exception and handle it as needed
                throw new Exception("An error occurred while deleting an item.", ex);
            }
        }
    }
}
