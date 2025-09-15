using ToDo.Core.DTOs;
using ToDo.Core.Interfaces.Services;

namespace ToDo.Service.Services
{
    public class ItemService: IItemService
    {
        private readonly IRabbitMqService _rabbitMqService;

        public ItemService(IRabbitMqService rabbitMqService)
        {
            _rabbitMqService = rabbitMqService;
        }

        public Task SendItemToQueueAsync(CreateItemRequest request)
        {
            var msg = new ItemMessageDto
            {
                Title = request.Title,
                Description = request.Description,
                UserId = request.UserId,
                Action = "Create"
            };

            return _rabbitMqService.PublishItemAsync(msg);
        }

        public Task SoftDeleteItemAsync(int itemId)
        {
            var msg = new ItemMessageDto
            {
                ItemId = itemId,
                Action = "Delete"
            };
            return _rabbitMqService.PublishItemAsync(msg);
        }

        public Task CompleteItemAsync(int itemId)
        {
            var msg = new ItemMessageDto
            {
                ItemId = itemId,
                Action = "Complete"
            };
            return _rabbitMqService.PublishItemAsync(msg);
        }
    }
}
