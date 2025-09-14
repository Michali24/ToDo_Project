using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo.Core.DTOs;
using ToDo.Core.Entities;
using ToDo.Core.Interfaces.Repositories;
using ToDo.Core.Interfaces.Services;

namespace ToDo.Service.Services
{
    public class ItemService: IItemService
    {
        private readonly IItemRepository _itemRepository;

        public ItemService(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        public async Task CompleteItemAsync(int itemId)
        {
            await _itemRepository.MarkItemAsCompletedAsync(itemId);
        }

        public async Task SoftDeleteItemAsync(int itemId)
        {
            await _itemRepository.SoftDeleteItemAsync(itemId);
        }

        public async Task HandleNewItemAsync(ItemMessageDto message)
        {
            // הגנות ברורות במקום !null-forgiving
            if (string.IsNullOrWhiteSpace(message.Title))
                throw new ArgumentException("Title is required for Create.");
            if (message.UserId <= 0)
                throw new ArgumentException("UserId must be positive for Create.");

            var item = new Item
            {
                Title = message.Title,
                Description = message.Description,
                UserId = message.UserId,
                IsCompleted = false,
                IsDeleted = false
            };

            await _itemRepository.AddItemAsync(item);
        }

    }
}
