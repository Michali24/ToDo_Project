using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using ToDo.Core.DTOs;
using ToDo.Core.Interfaces.Services;

namespace ToDo.WorkerService.Consumers
{
    public class UserConsumer:BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<UserConsumer> _logger;
        private IConnection _connection;
        private IModel _channel;

        public UserConsumer(IServiceProvider serviceProvider, ILogger<UserConsumer> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;

            var factory = new ConnectionFactory
            {
                HostName = "localhost" // אם זה רץ בדוקר, ודאי שזה תואם למה שכתוב ב־appsettings.json
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: "user_queue",
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🟢 UserConsumer is running...");

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                _logger.LogInformation("📥 Received message: {Message}", message);

                try
                {
                    var userDto = JsonSerializer.Deserialize<CreateUserRequest>(message);

                    if (userDto != null)
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                        await userService.HandleNewUserAsync(userDto);

                        _logger.LogInformation("✅ User handled and saved to DB.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Failed to process user message.");
                }
            };

            _channel.BasicConsume(queue: "user_queue",
                                  autoAck: true,
                                  consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }

    }
}
