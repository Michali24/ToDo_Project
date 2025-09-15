using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using ToDo.Core.DTOs;
using ToDo.Core.Interfaces.Services;
using ToDo.Core.Settings;

namespace ToDo.Service.Services
{
    public class RabbitMqService:IRabbitMqService
    {
        private readonly RabbitMqSettings _settings;
        private readonly ConnectionFactory _factory;

        public RabbitMqService(IOptions<RabbitMqSettings> options)
        {
            _settings = options.Value;
            _factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                UserName = _settings.Username,
                Password = _settings.Password
            };
        }

        public Task PublishItemAsync(ItemMessageDto message)
              => SendMessageAsync(message, _settings.ItemQueue);

        public Task PublishUserAsync(CreateUserRequest request)
            => SendMessageAsync(request, _settings.UserQueue);

        // Generic function for all actions
        public Task SendMessageAsync<T>(T message, string queueName)
        {
            //An open TCP connection between your app and the broker (RabbitMQ server).
            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();

            // If the queue not create
            channel.QueueDeclare(
                queue: queueName,
                durable: true,// The queue is kept even if RabbitMQ restarts.
                exclusive: false,// The queue is open to any connection.
                autoDelete: false,// The queue is not deleted automatically.
                arguments: null);

            // Serialize → bytes
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            // Message properties
            var props = channel.CreateBasicProperties();
            props.Persistent = true;
            props.ContentType = "application/json";//Lets consumers know the payload is in JSON format

            // Send the msg to RabbitMQ .
            channel.BasicPublish(
                exchange: "",
                routingKey: queueName,
                basicProperties: props,
                body: body);

            return Task.CompletedTask;

        }

    }
}
