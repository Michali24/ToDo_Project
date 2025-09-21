using ToDo.Core.DTOs;
using ToDo.Core.Interfaces.Services;

namespace ToDo.Service.Services
{
    public class ItemService: IItemService
    {
        //private readonly IRabbitMqService _rabbitMqService;

        //public ItemService(IRabbitMqService rabbitMqService)
        //{
        //    _rabbitMqService = rabbitMqService;
        //}

        //public Task SendItemToQueueAsync(CreateItemRequest request)
        //{
        //    var msg = new ItemMessageDto
        //    {
        //        Title = request.Title,
        //        Description = request.Description,
        //        UserId = request.UserId,
        //        Action = "Create"
        //    };

        //    return _rabbitMqService.PublishItemAsync(msg);
        //}

        //public Task SoftDeleteItemAsync(int itemId)
        //{
        //    var msg = new ItemMessageDto
        //    {
        //        ItemId = itemId,
        //        Action = "Delete"
        //    };
        //    return _rabbitMqService.PublishItemAsync(msg);
        //}

        //public Task CompleteItemAsync(int itemId)
        //{
        //    var msg = new ItemMessageDto
        //    {
        //        ItemId = itemId,
        //        Action = "Complete"
        //    };
        //    return _rabbitMqService.PublishItemAsync(msg);
        //}


        private readonly IRabbitMqRpc _rpc;

        public ItemService(IRabbitMqRpc rpc)
        {
            _rpc = rpc;
        }

        public async Task<OperationResponse> CreateItemAsync(CreateItemRequest request, CancellationToken ct = default)
        {
            // כאן בנינו את ההודעה עם Action במקום ב-Controller
            var msg = new ItemMessageDto
            {
                Title = request.Title,
                Description = request.Description,
                UserId = request.UserId,
                Action = "Create"
            };

            return await _rpc.RequestItemAsync<ItemMessageDto, OperationResponse>(
                payload: msg,
                timeout: TimeSpan.FromSeconds(10),
                ct: ct);
        }

        public async Task<OperationResponse> SoftDeleteItemAsync(int itemId, CancellationToken ct = default)
        {
            var msg = new ItemMessageDto { ItemId = itemId, Action = "Delete" };

            return await _rpc.RequestItemAsync<ItemMessageDto, OperationResponse>(
                payload: msg,
                timeout: TimeSpan.FromSeconds(10),
                ct: ct);
        }

        public async Task<OperationResponse> CompleteItemAsync(int itemId, CancellationToken ct = default)
        {
            var msg = new ItemMessageDto { ItemId = itemId, Action = "Complete" };

            return await _rpc.RequestItemAsync<ItemMessageDto, OperationResponse>(
                payload: msg,
                timeout: TimeSpan.FromSeconds(10),
                ct: ct);
        }
    
    }
}
