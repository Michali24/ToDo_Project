using ToDo.Core.DTOs;

namespace ToDo.Core.Interfaces.Services
{
    public interface IItemService
    {
        //Task HandleNewItemAsync(ItemMessageDto message);
        //Task CompleteItemAsync(int itemId);
        //Task SoftDeleteItemAsync(int itemId);


        Task<OperationResponse> HandleNewItemAsync(ItemMessageDto message);
        Task<OperationResponse> CompleteItemAsync(int itemId);
        Task<OperationResponse> SoftDeleteItemAsync(int itemId);

    }
}