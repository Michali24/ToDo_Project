using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
