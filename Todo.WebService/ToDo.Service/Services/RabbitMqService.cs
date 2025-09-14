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

        // עטיפות נוחות – אין לוגיקה בפנים, הכל עובר ל-SendMessageAsync
        //public Task PublishItemAsync(CreateItemRequest request)
        //    => SendMessageAsync(request, _settings.ItemQueue);

        public Task PublishItemAsync(ItemMessageDto message)
              => SendMessageAsync(message, _settings.ItemQueue);

        public Task PublishUserAsync(CreateUserRequest request)
            => SendMessageAsync(request, _settings.UserQueue);

        // מנוע כללי ומרוכז לכל השילוח
        public Task SendMessageAsync<T>(T message, string queueName)
        {
            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();

            // אם התור לא קיים – נוצר עכשיו
            channel.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            // Serialize → bytes
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            // סימון ההודעה כ-persistent + טיפוס תוכן
            var props = channel.CreateBasicProperties();
            props.Persistent = true;
            props.ContentType = "application/json";

            // שליחה (Default Exchange)
            channel.BasicPublish(
                exchange: "",
                routingKey: queueName,
                basicProperties: props,
                body: body);

            // אין await אמיתי כאן – שומרים על חתימה אסינכרונית
            return Task.CompletedTask;

        }

    }
}
