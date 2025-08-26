using Microsoft.EntityFrameworkCore.Metadata;
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

namespace ToDo.WorkerService.Consumers
{
    public class ItemConsumer : BackgroundService
    {
        //private readonly IServiceProvider _serviceProvider;
        //private readonly ILogger<ItemConsumer> _logger;
        //private IConnection _connection;
        //private RabbitMQ.Client.IModel _channel;

        //public ItemConsumer(IServiceProvider serviceProvider, ILogger<ItemConsumer> logger)
        //{
        //    _serviceProvider = serviceProvider;
        //    _logger = logger;

        //    var factory = new ConnectionFactory
        //    {
        //        HostName = "localhost" // בדוקר זה אמור להיות זהה לקובץ appsettings.json
        //    };

        //    _connection = factory.CreateConnection();
        //    _channel = _connection.CreateModel();

        //    _channel.QueueDeclare(queue: "item_queue",
        //                          durable: true,//התור נשמר גם אחרי הפעלה מחדש של RabbitMQ.
        //                          exclusive: false,//התור נגיש ליותר מ־1 צרכן.
        //                          autoDelete: false,//התור לא נמחק כשהלקוח מתנתק.
        //                          arguments: null);
        //}

        ////נקראת אוטומטית כשה־Worker עולה.
        ////כאן מתחיל המאזין (consumer) להאזין להודעות בתור.
        //protected override Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    _logger.LogInformation("🟢 ItemConsumer is running...");

        //    //זהו event-based consumer – כשמגיעה הודעה, הוא מפעיל את האירוע Received
        //    var consumer = new EventingBasicConsumer(_channel);

        //    consumer.Received += async (model, ea) =>
        //    {
        //        //בתוך האירוע:
        //        //שולף את ההודעה בפורמט מחרוזת.
        //        var body = ea.Body.ToArray();
        //        var message = Encoding.UTF8.GetString(body);

        //        _logger.LogInformation("📥 Received message: {Message}", message);

        //        try
        //        {
        //            // טיפול בהודעה
        //            //ההודעה מומרת לאובייקט מסוג CreateItemRequest
        //            var itemDto = JsonSerializer.Deserialize<CreateItemRequest>(message);

        //            if (itemDto != null)
        //            {
        //                //שמירה ל־DB
        //                //יוצרת טווח שירות (scope) כדי לקבל מופע של ItemService
        //                //קוראת למתודה ששומרת את האובייקט למסד הנתונים
        //                using var scope = _serviceProvider.CreateScope();
        //                var itemService = scope.ServiceProvider.GetRequiredService<IItemService>();
        //                await itemService.HandleNewItemAsync(itemDto);

        //                _logger.LogInformation("✅ Item handled and saved to DB.");
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError(ex, "❌ Failed to process item message.");
        //        }
        //    };
        //    // התחברות לתור
        //    //BasicConsume מתחיל את תהליך הקריאה מהתור
        //    //autoAck: true – ההודעה נחשבת "נטענה בהצלחה" מיד עם קבלתה
        //    _channel.BasicConsume(queue: "item_queue",
        //                          autoAck: true,
        //                          consumer: consumer);

        //    return Task.CompletedTask;
        //}

        ////Dispose
        ////סוגר את הערוץ והחיבור בסיום חיי השירות.
        //public override void Dispose()
        //{
        //    _channel?.Close();
        //    _connection?.Close();
        //    base.Dispose();
        //}

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ItemConsumer> _logger;
        private IConnection _connection;
        private RabbitMQ.Client.IModel _channel;

        public ItemConsumer(IServiceProvider serviceProvider, ILogger<ItemConsumer> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;

            var factory = new ConnectionFactory
            {
                HostName = "localhost" // או לפי appsettings.json אם יש לך שם הגדרה
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                queue: "item_queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🟢 ItemConsumer is running...");

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var messageString = Encoding.UTF8.GetString(body);

                _logger.LogInformation("📥 Received message: {Message}", messageString);

                try
                {
                    var message = JsonSerializer.Deserialize<ItemMessageDto>(messageString);

                    if (message != null)
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var itemService = scope.ServiceProvider.GetRequiredService<IItemService>();

                        switch (message.Action)
                        {
                            case "Create":
                                await itemService.HandleNewItemAsync(message);
                                _logger.LogInformation("✅ Item created and saved.");
                                break;

                            case "Complete":
                                await itemService.CompleteItemAsync(message.ItemId);
                                _logger.LogInformation("✅ Item marked as completed.");
                                break;

                            case "Delete":
                                await itemService.SoftDeleteItemAsync(message.ItemId);
                                _logger.LogInformation("✅ Item soft-deleted.");
                                break;

                            default:
                                _logger.LogWarning("⚠️ Unknown action: {Action}", message.Action);
                                break;
                        }
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

            _channel.BasicConsume(queue: "item_queue", autoAck: true, consumer: consumer);
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
