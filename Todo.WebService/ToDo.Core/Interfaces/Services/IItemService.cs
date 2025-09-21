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
        //Task SendItemToQueueAsync(CreateItemRequest request);
        //Task SoftDeleteItemAsync(int itemId);
        //Task CompleteItemAsync(int itemId);


        Task<OperationResponse> CreateItemAsync(CreateItemRequest request, CancellationToken ct = default);
        Task<OperationResponse> SoftDeleteItemAsync(int itemId, CancellationToken ct = default);
        Task<OperationResponse> CompleteItemAsync(int itemId, CancellationToken ct = default);
    }
}
