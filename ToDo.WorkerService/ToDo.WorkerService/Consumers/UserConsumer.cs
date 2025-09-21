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
        ////DI
        ////Open new scope
        //private readonly IServiceProvider _serviceProvider;
        ////Logger
        //private readonly ILogger<UserConsumer> _logger;
        ////Connectio TCP to RabbitMQ
        //private IConnection _connection;
        ////Channel
        //private IModel _channel;
        //private readonly RabbitMqSettings _settings;

        //public UserConsumer(IServiceProvider serviceProvider, ILogger<UserConsumer> logger, IOptions<RabbitMqSettings> options)
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

        //    _channel.QueueDeclare(queue: _settings.UserQueue,
        //                          durable: true, // The queue is kept even if RabbitMQ restarts.
        //                          exclusive: false,// The queue is open to any connection.
        //                          autoDelete: false,// The queue is not deleted automatically.
        //                          arguments: null); 
        //}

        //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    _logger.LogInformation("🟢 UserConsumer is running...");

        //    var consumer = new EventingBasicConsumer(_channel);//Create new consumer 

        //    consumer.Received += async (model, ea) => //model->Information about the queue , ea->obj with the msg from RabbitMq
        //    {
        //        var body = ea.Body.ToArray();//Convert a byte array to JSON
        //        var message = Encoding.UTF8.GetString(body);//Convert this array to a String

        //        _logger.LogInformation("📥 Received message: {Message}", message);

        //        try
        //        {
        //            var userDto = JsonSerializer.Deserialize<CreateUserRequest>(message);//Deserialize the array to userDto

        //            if (userDto != null)
        //            {
        //                using var scope = _serviceProvider.CreateScope();//Create Scope beacuse we don't have something that carete it
        //                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        //                await userService.HandleNewUserAsync(userDto);

        //                _logger.LogInformation("✅ User handled and saved to DB.");
        //                _channel.BasicAck(deliveryTag: ea.DeliveryTag,//Marks a message as delivered to the customer
        //                                  multiple: false);//Confirms only this specific message!
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError(ex, "❌ Failed to process user message.");
        //        }
        //    };
        //    //Conect the consumer to queue 
        //    _channel.BasicConsume(queue: _settings.UserQueue,
        //                          //autoAck: true,//
        //                          autoAck:false,//Don't delete the msg only after the code send Ack, still in unacked.
        //                                        //and if the Worker done the msg don't lose.
        //                          consumer: consumer);

        //    await Task.Delay(Timeout.Infinite, stoppingToken);//The server still live until it's need to stop
        //}

        //public override void Dispose()
        //{
        //    _channel?.Close();//Close the Channel
        //    _connection?.Close();//Close the Connection
        //    base.Dispose();//Clean more things from the BackgroundService
        //}


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //private readonly IServiceProvider _serviceProvider;
        //private readonly ILogger<UserConsumer> _logger;
        //private readonly RabbitMqSettings _settings;

        //private IConnection _connection;
        //private IModel _channel;

        //public UserConsumer(IServiceProvider serviceProvider, ILogger<UserConsumer> logger, IOptions<RabbitMqSettings> options)
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

        //    _channel.QueueDeclare(
        //        queue: _settings.UserQueue,
        //        durable: true,
        //        exclusive: false,
        //        autoDelete: false,
        //        arguments: null);
        //}

        //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    _logger.LogInformation("🟢 UserConsumer is running and waiting for messages...");

        //    var consumer = new EventingBasicConsumer(_channel);

        //    consumer.Received += async (model, ea) =>
        //    {
        //        var messageBody = Encoding.UTF8.GetString(ea.Body.ToArray());
        //        _logger.LogInformation("📥 Received message on UserQueue: {Message}", messageBody);

        //        OperationResponse response;

        //        try
        //        {
        //            var userRequest = JsonSerializer.Deserialize<CreateUserRequest>(messageBody);

        //            if (userRequest != null)
        //            {
        //                using var scope = _serviceProvider.CreateScope();
        //                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

        //                response = await userService.HandleNewUserAsync(userRequest);
        //            }
        //            else
        //            {
        //                response = new OperationResponse
        //                {
        //                    Success = false,
        //                    Message = "Invalid User message format ❌",
        //                    ExecutedAt = DateTime.UtcNow
        //                };
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError(ex, "❌ Failed to process User message.");
        //            response = new OperationResponse
        //            {
        //                Success = false,
        //                ErrorMessage = ex.Message,
        //                Message = "Failed to process user ❌",
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

        //            _logger.LogInformation("📤 Sent RPC response for User (CorrelationId={CorrId})", replyProps.CorrelationId);

        //            _channel.BasicAck(ea.DeliveryTag, false);
        //        }
        //        catch (Exception sendEx)
        //        {
        //            _logger.LogError(sendEx, "❌ Failed to send User RPC response.");
        //        }
        //    };

        //    _channel.BasicConsume(
        //        queue: _settings.UserQueue,
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
        private readonly ILogger<UserConsumer> _logger;
        private readonly RabbitMqSettings _settings;

        private IConnection _connection;
        private IModel _channel;

        public UserConsumer(IServiceProvider serviceProvider, ILogger<UserConsumer> logger, IOptions<RabbitMqSettings> options)
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

            // וידוא שהתור קיים (idempotent)
            _channel.QueueDeclare(
                queue: _settings.UserQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🟢 UserConsumer is running and waiting for messages...");

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                var messageBody = Encoding.UTF8.GetString(ea.Body.ToArray());
                _logger.LogInformation("📥 Received message on UserQueue: {Message}", messageBody);

                OperationResponse response;

                try
                {
                    var userRequest = JsonSerializer.Deserialize<CreateUserRequest>(messageBody);

                    if (userRequest != null)
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

                        response = await userService.HandleNewUserAsync(userRequest);

                        _logger.LogInformation("✅ User created successfully (Name={Name})", userRequest.Name);
                    }
                    else
                    {
                        response = new OperationResponse
                        {
                            Success = false,
                            Message = "Invalid User message format ❌",
                            ExecutedAt = DateTime.UtcNow
                        };

                        _logger.LogWarning("⚠️ Failed to deserialize user message.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Failed to process User message.");
                    response = new OperationResponse
                    {
                        Success = false,
                        ErrorMessage = ex.Message,
                        Message = "Failed to process user ❌",
                        ExecutedAt = DateTime.UtcNow
                    };
                }

                // שליחת תשובה ל-ReplyTo עם CorrelationId
                try
                {
                    var replyProps = _channel.CreateBasicProperties();
                    replyProps.CorrelationId = ea.BasicProperties.CorrelationId;

                    var replyBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));

                    _channel.BasicPublish(
                        exchange: "",
                        routingKey: ea.BasicProperties.ReplyTo,
                        basicProperties: replyProps,
                        body: replyBody);

                    _logger.LogInformation("📤 Sent RPC response for User (CorrelationId={CorrId})", replyProps.CorrelationId);

                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception sendEx)
                {
                    _logger.LogError(sendEx, "❌ Failed to send User RPC response.");
                }
            };

            _channel.BasicConsume(
                queue: _settings.UserQueue,
                autoAck: false,
                consumer: consumer);

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
