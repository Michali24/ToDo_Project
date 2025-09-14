using ToDo.Core.DTOs;

namespace ToDo.Core.Interfaces.Services
{
    public interface IItemService
    {
        //Task HandleNewItemAsync(CreateItemRequest request);
        Task HandleNewItemAsync(ItemMessageDto message);
        Task CompleteItemAsync(int itemId);
        Task SoftDeleteItemAsync(int itemId);
    }
}