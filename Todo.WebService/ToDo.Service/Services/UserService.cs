using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo.Core.DTOs;
using ToDo.Core.Entities;
using ToDo.Core.Interfaces.Repositories;
using ToDo.Core.Interfaces.Services;

namespace ToDo.Service.Services
{
    public class UserService: IUserService
    {
        private readonly IRabbitMqService _rabbitMqService;

        public UserService(IRabbitMqService rabbitMqService)
        {
            _rabbitMqService = rabbitMqService;
        }

        public async Task SendUserToQueueAsync(CreateUserRequest request)
        {
            // שליחה ל-RabbitMQ בלבד – לא שומר ל-DB
            await _rabbitMqService.PublishUserAsync(request);
        }
    }
}
