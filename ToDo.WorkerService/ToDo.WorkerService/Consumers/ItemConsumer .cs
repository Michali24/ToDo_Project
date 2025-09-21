//using Microsoft.EntityFrameworkCore.Metadata;
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
        ////DI
        ////Open new scope
        //private readonly IServiceProvider _serviceProvider;
        ////Logger
        //private readonly ILogger<ItemConsumer> _logger;
        ////Connectio TCP to RabbitMQ
        //private IConnection _connection;
        ////Channel
        //private RabbitMQ.Client.IModel _channel;

        //private readonly RabbitMqSettings _settings;


        //public ItemConsumer(IServiceProvider serviceProvider, ILogger<ItemConsumer> logger, IOptions<RabbitMqSettings> options)
        //{
        //    _serviceProvider = serviceProvider;
        //    _logger = logger;
        //    _settings = options.Value;

        //    //Object of RabbitMQ that open Connections
        //    var factory = new ConnectionFactory
        //    {
        //        HostName = _settings.HostName,
        //        UserName = _settings.Username,
        //        Password = _settings.Password
        //    };

        //    //An open TCP connection between your app and the broker (RabbitMQ server).
        //    _connection = factory.CreateConnection();
        //    _channel = _connection.CreateModel();

        //    _channel.QueueDeclare(
        //        queue: _settings.ItemQueue,
        //        durable: true,// The queue is kept even if RabbitMQ restarts.
        //        exclusive: false,// The queue is open to any connection.
        //        autoDelete: false,// The queue is not deleted automatically.
        //        arguments: null
        //    );
        //}

        //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    _logger.LogInformation("🟢 ItemConsumer is running...");

        //    var consumer = new EventingBasicConsumer(_channel);//Create new consumer

        //    consumer.Received += async (model, ea) =>//model->Information about the queue , ea->obj with the msg from RabbitMq
        //    {
        //        var body = ea.Body.ToArray();//Convert a byte array to JSON
        //        var messageString = Encoding.UTF8.GetString(body);//Convert this array to a String

        //        _logger.LogInformation("📥 Received message: {Message}", messageString);

        //        try
        //        {
        //            var message = JsonSerializer.Deserialize<ItemMessageDto>(messageString);//Deserialize the array to userDto

        //            if (message != null)
        //            {
        //                using var scope = _serviceProvider.CreateScope();//Create Scope beacuse we don't have something that carete it
        //                var itemService = scope.ServiceProvider.GetRequiredService<IItemService>();

        //                switch (message.Action)
        //                {
        //                    case "Create":
        //                        await itemService.HandleNewItemAsync(message);//Create new Item
        //                        _logger.LogInformation("✅ Item created and saved.");
        //                        break;

        //                    case "Complete":
        //                        await itemService.CompleteItemAsync(message.ItemId);//Complete Item
        //                        _logger.LogInformation("✅ Item marked as completed.");
        //                        break;

        //                    case "Delete":
        //                        await itemService.SoftDeleteItemAsync(message.ItemId);//SoftDelete Item
        //                        _logger.LogInformation("✅ Item soft-deleted.");
        //                        break;

        //                    default:
        //                        _logger.LogWarning("⚠️ Unknown action: {Action}", message.Action);
        //                        break;
        //                }
        //                _channel.BasicAck(deliveryTag: ea.DeliveryTag,//Marks a message as delivered to the customer
        //                                  multiple: false);//Confirms only this specific message!
        //            }
        //            else
        //            {
        //                _logger.LogWarning("⚠️ Could not deserialize message.");
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError(ex, "❌ Failed to process item message.");
        //        }
        //    };

        //    //Conect the consumer to queue 
        //    _channel.BasicConsume(queue: _settings.ItemQueue,
        //                          //autoAck: true,//
        //                          autoAck: false,//Don't delete the msg only after the code send Ack.
        //                                         //and if the Worker done the msg don't lose.
        //                          consumer: consumer);

        //    //return Task.CompletedTask;
        //    await Task.Delay(Timeout.Infinite, stoppingToken);
        //}

        //public override void Dispose()
        //{
        //    _channel?.Close();//Close the Channel.
        //    _connection?.Close();//Close the Connection
        //    base.Dispose();//Clean more things from the BackgroundService
        //}


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        //private readonly IServiceProvider _serviceProvider;
        //private readonly ILogger<ItemConsumer> _logger;
        //private readonly RabbitMqSettings _settings;

        //private IConnection _connection;
        //private IModel _channel;

        //public ItemConsumer(IServiceProvider serviceProvider, ILogger<ItemConsumer> logger, IOptions<RabbitMqSettings> options)
        //{
        //    _serviceProvider = serviceProvider;
        //    _logger = logger;
        //    _settings = options.Value;

        //    var factory = new ConnectionFactory
        //    {
        //        HostName = _settings.HostName,
        //        UserName = _settings.Username,
        //        Password = _settings.Password
        //    };

        //    _connection = factory.CreateConnection();
        //    _channel = _connection.CreateModel();

        //    // מוודאים שהתור קיים (idempotent)
        //    _channel.QueueDeclare(
        //        queue: _settings.ItemQueue,
        //        durable: true,
        //        exclusive: false,
        //        autoDelete: false,
        //        arguments: null);
        //}

        //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    _logger.LogInformation("🟢 ItemConsumer is running and waiting for messages...");

        //    var consumer = new EventingBasicConsumer(_channel);

        //    consumer.Received += async (model, ea) =>
        //    {
        //        var messageBody = Encoding.UTF8.GetString(ea.Body.ToArray());
        //        _logger.LogInformation("📥 Received message on ItemQueue: {Message}", messageBody);

        //        OperationResponse response;

        //        try
        //        {
        //            var message = JsonSerializer.Deserialize<ItemMessageDto>(messageBody);

        //            if (message != null)
        //            {
        //                _logger.LogWarning("⚠️ Could not deserialize Item message.");

        //                using var scope = _serviceProvider.CreateScope();
        //                var itemService = scope.ServiceProvider.GetRequiredService<IItemService>();

        //                switch (message.Action)
        //                {
        //                    case "Create":
        //                        response = await itemService.HandleNewItemAsync(message);
        //                        _logger.LogInformation("✅ Item created successfully (Title={Title}, UserId={UserId})",
        //                        message.Title, message.UserId);
        //                        break;
        //                    case "Complete":
        //                        response = await itemService.CompleteItemAsync(message.ItemId);
        //                        _logger.LogInformation("✅ Item marked as completed (ItemId={ItemId})", message.ItemId);
        //                        break;
        //                    case "Delete":
        //                        response = await itemService.SoftDeleteItemAsync(message.ItemId);
        //                        _logger.LogInformation("✅ Item soft-deleted (ItemId={ItemId})", message.ItemId);
        //                        break;
        //                    default:
        //                        response = new OperationResponse
        //                        {
        //                            Success = false,
        //                            Message = $"Unknown action: {message.Action}",
        //                            ExecutedAt = DateTime.UtcNow
        //                        };
        //                        _logger.LogWarning("⚠️ Unknown action: {Action}", message.Action);
        //                        break;
        //                }
        //            }
        //            else
        //            {
        //                response = new OperationResponse
        //                {
        //                    Success = false,
        //                    Message = "Invalid Item message format ❌",
        //                    ExecutedAt = DateTime.UtcNow
        //                };
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError(ex, "❌ Failed to process Item message.");
        //            response = new OperationResponse
        //            {
        //                Success = false,
        //                ErrorMessage = ex.Message,
        //                Message = "Failed to process item ❌",
        //                ExecutedAt = DateTime.UtcNow
        //            };
        //        }

        //        // שליחת תשובה ל-ReplyTo עם CorrelationId
        //        try
        //        {
        //            var replyProps = _channel.CreateBasicProperties();
        //            replyProps.CorrelationId = ea.BasicProperties.CorrelationId;

        //            var replyBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));

        //            _channel.BasicPublish(
        //                exchange: "",
        //                routingKey: ea.BasicProperties.ReplyTo,
        //                basicProperties: replyProps,
        //                body: replyBody);

        //            _logger.LogInformation("📤 Sent RPC response for Item (CorrelationId={CorrId})", replyProps.CorrelationId);

        //            _channel.BasicAck(ea.DeliveryTag, false);
        //        }
        //        catch (Exception sendEx)
        //        {
        //            _logger.LogError(sendEx, "❌ Failed to send Item RPC response.");
        //        }
        //    };

        //    _channel.BasicConsume(
        //        queue: _settings.ItemQueue,
        //        autoAck: false,
        //        consumer: consumer);

        //    await Task.Delay(Timeout.Infinite, stoppingToken);
        //}

        //public override void Dispose()
        //{
        //    _channel?.Close();
        //    _connection?.Close();
        //    base.Dispose();
        //}




        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ItemConsumer> _logger;
        private readonly RabbitMqSettings _settings;

        private IConnection _connection;
        private IModel _channel;

        public ItemConsumer(IServiceProvider serviceProvider, ILogger<ItemConsumer> logger, IOptions<RabbitMqSettings> options)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _settings = options.Value;

            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                UserName = _settings.Username,
                Password = _settings.Password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                queue: _settings.ItemQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🟢 ItemConsumer is running and waiting for messages...");

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                var messageBody = Encoding.UTF8.GetString(ea.Body.ToArray());
                _logger.LogInformation("📥 Received message on ItemQueue: {Message}", messageBody);

                OperationResponse response;

                try
                {
                    var message = JsonSerializer.Deserialize<ItemMessageDto>(messageBody);

                    if (message == null)
                    {
                        _logger.LogWarning("⚠️ Could not deserialize Item message.");
                        response = new OperationResponse
                        {
                            Success = false,
                            Message = "Invalid Item message format ❌",
                            ExecutedAt = DateTime.UtcNow
                        };
                    }
                    else
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var itemService = scope.ServiceProvider.GetRequiredService<IItemService>();

                        switch (message.Action)
                        {
                            case "Create":
                                _logger.LogInformation("🛠 Handling Create Item request...");
                                await itemService.HandleNewItemAsync(message);
                                response = new OperationResponse
                                {
                                    Success = true,
                                    Id = message.ItemId,
                                    Message = $"Item created successfully ✅",
                                    ExecutedAt = DateTime.UtcNow
                                };
                                break;

                            case "Complete":
                                _logger.LogInformation("🛠 Handling Complete Item request...");
                                await itemService.CompleteItemAsync(message.ItemId);
                                response = new OperationResponse
                                {
                                    Success = true,
                                    Id = message.ItemId,
                                    Message = $"Item marked as completed ✅",
                                    ExecutedAt = DateTime.UtcNow
                                };
                                break;

                            case "Delete":
                                _logger.LogInformation("🛠 Handling Delete Item request...");
                                await itemService.SoftDeleteItemAsync(message.ItemId);
                                response = new OperationResponse
                                {
                                    Success = true,
                                    Id = message.ItemId,
                                    Message = $"Item soft-deleted 🗑",
                                    ExecutedAt = DateTime.UtcNow
                                };
                                break;

                            default:
                                _logger.LogWarning("⚠️ Unknown action: {Action}", message.Action);
                                response = new OperationResponse
                                {
                                    Success = false,
                                    Message = $"Unknown action: {message.Action}",
                                    ExecutedAt = DateTime.UtcNow
                                };
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Failed to process Item message.");
                    response = new OperationResponse
                    {
                        Success = false,
                        ErrorMessage = ex.Message,
                        Message = "Failed to process Item ❌",
                        ExecutedAt = DateTime.UtcNow
                    };
                }

                // ✅ שליחת תשובת RPC חזרה ל-ReplyTo (אם קיים)
                try
                {
                    if (!string.IsNullOrEmpty(ea.BasicProperties.ReplyTo))
                    {
                        var replyProps = _channel.CreateBasicProperties();
                        replyProps.CorrelationId = ea.BasicProperties.CorrelationId;

                        var replyBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));

                        _channel.BasicPublish(
                            exchange: "",
                            routingKey: ea.BasicProperties.ReplyTo,
                            basicProperties: replyProps,
                            body: replyBody);

                        _logger.LogInformation("📤 Sent RPC response for Item (CorrelationId={CorrId})", replyProps.CorrelationId);
                    }
                    else
                    {
                        _logger.LogWarning("⚠️ No ReplyTo found – message processed but no RPC response sent.");
                    }

                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception sendEx)
                {
                    _logger.LogError(sendEx, "❌ Failed to send RPC response for Item.");
                }
            };

            _channel.BasicConsume(queue: _settings.ItemQueue, autoAck: false, consumer: consumer);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}
