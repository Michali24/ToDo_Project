using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using ToDo.Core.DTOs;
using ToDo.Core.Entities;
using ToDo.Core.Interfaces.Repositories;
using ToDo.Core.Interfaces.Services;
using ToDo.Core.Settings;

namespace ToDo.Service.Services
{ 
    public class ItemService: IItemService
    {
        //private readonly IRabbitMqService _rabbitMqService;
        //private readonly RabbitMqSettings _settings;


        //public ItemService(IRabbitMqService rabbitMqService, IOptions<RabbitMqSettings> options)
        //{
        //    _rabbitMqService = rabbitMqService;
        //    _settings = options.Value;


        //}

        //public async Task SendItemToQueueAsync(CreateItemRequest request)
        //{
        //    // שליחה ל-RabbitMQ בלבד – לא נוגעים במסד נתונים
        //    await _rabbitMqService.PublishItemAsync(request);
        //}

        //public async Task SoftDeleteItemAsync(int id)
        //{
        //    var message = new
        //    {
        //        Id = id,
        //        Action = "Delete"
        //    };

        //    await _rabbitMqService.SendMessageAsync(message, _settings.ItemQueue);
        //}

        //public async Task CompleteItemAsync(int id)
        //{
        //    var message = new
        //    {
        //        Id = id,
        //        Action = "Complete"
        //    };

        //    await _rabbitMqService.SendMessageAsync(message, _settings.ItemQueue);
        //}

        private readonly IRabbitMqService _mq;
        private readonly string _itemQueueName;

        public ItemService(IRabbitMqService mq, Microsoft.Extensions.Options.IOptions<ToDo.Core.Settings.RabbitMqSettings> opts)
        {
            _mq = mq;
            _itemQueueName = opts.Value.ItemQueue;
        }

        public Task SendItemToQueueAsync(CreateItemRequest request)
        {
            // בונים הודעה "שפת Worker"
            var msg = new ItemMessageDto
            {
                Title = request.Title,
                Description = request.Description,
                UserId = request.UserId,
                Action = "Create"
            };

            // שולחים דרך התשתית
            return _mq.SendMessageAsync(msg, _itemQueueName);
        }

        public Task SoftDeleteItemAsync(int itemId)
        {
            var msg = new ItemMessageDto
            {
                ItemId = itemId,
                Action = "Delete"
            };
            return _mq.SendMessageAsync(msg, _itemQueueName);
        }

        public Task CompleteItemAsync(int itemId)
        {
            var msg = new ItemMessageDto
            {
                ItemId = itemId,
                Action = "Complete"
            };
            return _mq.SendMessageAsync(msg, _itemQueueName);
        }

    }
}
