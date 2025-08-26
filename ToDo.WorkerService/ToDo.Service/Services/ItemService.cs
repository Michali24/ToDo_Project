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

        //public async Task HandleNewItemAsync(CreateItemRequest request)
        //{
        //    var item = new Item
        //    {
        //        Title = request.Title,
        //        Description = request.Description,
        //        IsCompleted = false,
        //        UserId = request.UserId
        //    };

        //    await _itemRepository.AddItemAsync(item);
        //}

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
            var item = new Item
            {
                Title = message.Title!,
                Description = message.Description,
                UserId = message.UserId,
                IsCompleted = false,
                IsDeleted = false
            };

            await _itemRepository.AddItemAsync(item);
        }

    }
}
