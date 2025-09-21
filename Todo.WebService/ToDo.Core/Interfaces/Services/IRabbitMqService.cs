//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using ToDo.Core.DTOs;

//namespace ToDo.Core.Interfaces.Services
//{
//    public interface IRabbitMqService
//    {
//        Task PublishItemAsync(ItemMessageDto message);//Sending a request to RabbitMQ for a task (Item)
//        Task PublishUserAsync(CreateUserRequest request);//Sending a request to RabbitMQ for a user
//        Task SendMessageAsync<T>(T message, string queueName);//A generic function for sending data to RabbitMQ

//    }
//}
