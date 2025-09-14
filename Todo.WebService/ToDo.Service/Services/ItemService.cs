using ToDo.Core.DTOs;
using ToDo.Core.Interfaces.Services;

namespace ToDo.Service.Services
{
    public class ItemService: IItemService
    {
        private readonly IRabbitMqService _rabbitMqService;
        //private readonly string _itemQueueName;

        //public ItemService(IRabbitMqService mq, Microsoft.Extensions.Options.IOptions<ToDo.Core.Settings.RabbitMqSettings> opts)
        //{
        //    _mq = mq;
        //    _itemQueueName = opts.Value.ItemQueue;
        //}

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

        //אפשר לכתוב גם בצורה הוז:
        //public Task SoftDeleteItemAsync(int itemId)
        //=> _mq.PublishItemAsync(new ItemMessageDto { ItemId = itemId, Action = "Delete" });

        //public Task CompleteItemAsync(int itemId)
        //    => _mq.PublishItemAsync(new ItemMessageDto { ItemId = itemId, Action = "Complete" });

    }
}
