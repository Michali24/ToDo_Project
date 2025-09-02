using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo.Core.DTOs;

namespace ToDo.Core.Interfaces.Services
{
    public interface IRabbitMqService
    {
        Task PublishItemAsync(CreateItemRequest request);
        Task PublishUserAsync(CreateUserRequest request);
        Task SendMessageAsync<T>(T message, string queueName);

    }
}
