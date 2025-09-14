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
using ToDo.Core.Settings;
using Microsoft.Extensions.Options;

namespace ToDo.WorkerService.Consumers
{
    public class UserConsumer:BackgroundService
    {
        //DI
        //Open new scope
        private readonly IServiceProvider _serviceProvider;
        //Logger
        private readonly ILogger<UserConsumer> _logger;
        //Connectio TCP to RabbitMQ
        private IConnection _connection;
        //Channel
        private IModel _channel;
        private readonly RabbitMqSettings _settings;

        public UserConsumer(IServiceProvider serviceProvider, ILogger<UserConsumer> logger, IOptions<RabbitMqSettings> options)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _settings = options.Value;

            //Object of RabbitMQ that open Connections
            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                UserName = _settings.Username,
                Password = _settings.Password
            };

            //An open TCP connection between your app and the broker (RabbitMQ server).
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: _settings.UserQueue,
                                  durable: true, // The queue is kept even if RabbitMQ restarts.
                                  exclusive: false,// The queue is open to any connection.
                                  autoDelete: false,// The queue is not deleted automatically.
                                  arguments: null); 
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🟢 UserConsumer is running...");

            var consumer = new EventingBasicConsumer(_channel);//Create new consumer 

            consumer.Received += async (model, ea) => //model->Information about the queue , ea->obj with the msg from RabbitMq
            {
                var body = ea.Body.ToArray();//Convert a byte array to JSON
                var message = Encoding.UTF8.GetString(body);//Convert this array to a String

                _logger.LogInformation("📥 Received message: {Message}", message);

                try
                {
                    var userDto = JsonSerializer.Deserialize<CreateUserRequest>(message);//Deserialize the array to userDto

                    if (userDto != null)
                    {
                        using var scope = _serviceProvider.CreateScope();//Create Scope beacuse we don't have something that carete it
                        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                        await userService.HandleNewUserAsync(userDto);

                        _logger.LogInformation("✅ User handled and saved to DB.");
                        _channel.BasicAck(deliveryTag: ea.DeliveryTag,//Marks a message as delivered to the customer
                                          multiple: false);//Confirms only this specific message!
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Failed to process user message.");
                }
            };
            //Conect the consumer to queue 
            _channel.BasicConsume(queue: _settings.UserQueue,
                                  //autoAck: true,//
                                  autoAck:false,//Don't delete the msg only after the code send Ack, still in unacked.
                                                //and if the Worker done the msg don't lose.
                                  consumer: consumer);

            await Task.Delay(Timeout.Infinite, stoppingToken);//The server still live until it's need to stop
        }

        public override void Dispose()
        {
            _channel?.Close();//Close the Channel
            _connection?.Close();//Close the Connection
            base.Dispose();//Clean more things from the BackgroundService
        }

    }
}
