using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ToDo.Core.DTOs;
using ToDo.Core.Interfaces.Services;
using ToDo.Core.Settings;

namespace ToDo.WorkerService.Consumers
{
    public class ItemConsumer : BackgroundService
    {
        //DI
        //Open new scope
        private readonly IServiceProvider _serviceProvider;
        //Logger
        private readonly ILogger<ItemConsumer> _logger;
        //Connectio TCP to RabbitMQ
        private IConnection _connection;
        //Channel
        private RabbitMQ.Client.IModel _channel;

        private readonly RabbitMqSettings _settings;


        public ItemConsumer(IServiceProvider serviceProvider, ILogger<ItemConsumer> logger, IOptions<RabbitMqSettings> options)
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

            _channel.QueueDeclare(
                queue: _settings.ItemQueue,
                durable: true,// The queue is kept even if RabbitMQ restarts.
                exclusive: false,// The queue is open to any connection.
                autoDelete: false,// The queue is not deleted automatically.
                arguments: null
            );
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🟢 ItemConsumer is running...");

            var consumer = new EventingBasicConsumer(_channel);//Create new consumer

            consumer.Received += async (model, ea) =>//model->Information about the queue , ea->obj with the msg from RabbitMq
            {
                var body = ea.Body.ToArray();//Convert a byte array to JSON
                var messageString = Encoding.UTF8.GetString(body);//Convert this array to a String

                _logger.LogInformation("📥 Received message: {Message}", messageString);

                try
                {
                    var message = JsonSerializer.Deserialize<ItemMessageDto>(messageString);//Deserialize the array to userDto

                    if (message != null)
                    {
                        using var scope = _serviceProvider.CreateScope();//Create Scope beacuse we don't have something that carete it
                        var itemService = scope.ServiceProvider.GetRequiredService<IItemService>();

                        switch (message.Action)
                        {
                            case "Create":
                                await itemService.HandleNewItemAsync(message);//Create new Item
                                _logger.LogInformation("✅ Item created and saved.");
                                break;

                            case "Complete":
                                await itemService.CompleteItemAsync(message.ItemId);//Complete Item
                                _logger.LogInformation("✅ Item marked as completed.");
                                break;

                            case "Delete":
                                await itemService.SoftDeleteItemAsync(message.ItemId);//SoftDelete Item
                                _logger.LogInformation("✅ Item soft-deleted.");
                                break;

                            default:
                                _logger.LogWarning("⚠️ Unknown action: {Action}", message.Action);
                                break;
                        }
                        _channel.BasicAck(deliveryTag: ea.DeliveryTag,//Marks a message as delivered to the customer
                                          multiple: false);//Confirms only this specific message!
                    }
                    else
                    {
                        _logger.LogWarning("⚠️ Could not deserialize message.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Failed to process item message.");
                }
            };

            //Conect the consumer to queue 
            _channel.BasicConsume(queue: _settings.ItemQueue,
                                  //autoAck: true,//
                                  autoAck: false,//Don't delete the msg only after the code send Ack.
                                                 //and if the Worker done the msg don't lose.
                                  consumer: consumer);

            //return Task.CompletedTask;
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        public override void Dispose()
        {
            _channel?.Close();//Close the Channel
            _connection?.Close();//Close the Connection
            base.Dispose();//Clean more things from the BackgroundService
        }

    }
}
