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
        private readonly IRabbitMqService _rabbitMqService;
        private readonly RabbitMqSettings _settings;


        public ItemService(IRabbitMqService rabbitMqService, IOptions<RabbitMqSettings> options)
        {
            _rabbitMqService = rabbitMqService;
            _settings = options.Value;


        }

        public async Task SendItemToQueueAsync(CreateItemRequest request)
        {
            // שליחה ל-RabbitMQ בלבד – לא נוגעים במסד נתונים
            await _rabbitMqService.PublishItemAsync(request);
        }

        public async Task SoftDeleteItemAsync(int id)
        {
            var message = new
            {
                Id = id,
                Action = "Delete"
            };

            await _rabbitMqService.SendMessageAsync(message, _settings.ItemQueue);
        }

        public async Task CompleteItemAsync(int id)
        {
            var message = new
            {
                Id = id,
                Action = "Complete"
            };

            await _rabbitMqService.SendMessageAsync(message, _settings.ItemQueue);
        }

    }
}
