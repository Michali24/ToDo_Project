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
        //private readonly IItemRepository _itemRepository;

        //public ItemService(IItemRepository itemRepository)
        //{
        //    _itemRepository = itemRepository;
        //}

        //public async Task CompleteItemAsync(int itemId)
        //{
        //    await _itemRepository.MarkItemAsCompletedAsync(itemId);
        //}

        //public async Task SoftDeleteItemAsync(int itemId)
        //{
        //    await _itemRepository.SoftDeleteItemAsync(itemId);
        //}

        //public async Task HandleNewItemAsync(ItemMessageDto message)
        //{
        //    if (string.IsNullOrWhiteSpace(message.Title))
        //        throw new ArgumentException("Title is required for Create.");
        //    if (message.UserId <= 0)
        //        throw new ArgumentException("UserId must be positive for Create.");

        //    var item = new Item
        //    {
        //        Title = message.Title,
        //        Description = message.Description,
        //        UserId = message.UserId,
        //        IsCompleted = false,
        //        IsDeleted = false
        //    };

        //    await _itemRepository.AddItemAsync(item);
        //}

        private readonly IItemRepository _itemRepository;

        public ItemService(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        public async Task<OperationResponse> HandleNewItemAsync(ItemMessageDto message)
        {
            try
            {
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

                return new OperationResponse
                {
                    Id = item.Id,
                    Success = true,
                    Message = $"Item '{item.Title}' created successfully ✅",
                    ExecutedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                return new OperationResponse
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    Message = "Failed to create item ❌",
                    ExecutedAt = DateTime.UtcNow
                };
            }
        }

        public async Task<OperationResponse> CompleteItemAsync(int itemId)
        {
            try
            {
                await _itemRepository.MarkItemAsCompletedAsync(itemId);
                return new OperationResponse
                {
                    Id = itemId,
                    Success = true,
                    Message = $"Item {itemId} marked as completed ✅",
                    ExecutedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                return new OperationResponse
                {
                    Id = itemId,
                    Success = false,
                    ErrorMessage = ex.Message,
                    Message = "Failed to complete item ❌",
                    ExecutedAt = DateTime.UtcNow
                };
            }
        }

        public async Task<OperationResponse> SoftDeleteItemAsync(int itemId)
        {
            try
            {
                await _itemRepository.SoftDeleteItemAsync(itemId);
                return new OperationResponse
                {
                    Id = itemId,
                    Success = true,
                    Message = $"Item {itemId} soft-deleted 🗑",
                    ExecutedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                return new OperationResponse
                {
                    Id = itemId,
                    Success = false,
                    ErrorMessage = ex.Message,
                    Message = "Failed to delete item ❌",
                    ExecutedAt = DateTime.UtcNow
                };
            }
        }

    }
}
