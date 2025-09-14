using ToDo.Core.Entities;

namespace ToDo.Core.Interfaces.Repositories
{
    public interface IItemRepository
    {
        Task AddItemAsync(Item item);
        Task MarkItemAsCompletedAsync(int itemId);
        Task SoftDeleteItemAsync(int itemId);
    }
}
