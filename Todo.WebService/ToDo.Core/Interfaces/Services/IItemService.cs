using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo.Core.DTOs;

namespace ToDo.Core.Interfaces.Services
{
    public interface IItemService
    {
        Task SendItemToQueueAsync(CreateItemRequest request);
        Task SoftDeleteItemAsync(int itemId);
        Task CompleteItemAsync(int itemId);
    }
}
