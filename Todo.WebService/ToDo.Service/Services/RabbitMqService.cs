using Microsoft.Extensions.Configuration;
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
        //private readonly string _hostname;
        //private readonly string _queueName;

        //private readonly RabbitMqSettings _settings;
        //private readonly ConnectionFactory _factory;


        //public RabbitMqService(IOptions<RabbitMqSettings> options)
        //{
        //    _settings = options.Value;
        //    _factory = new ConnectionFactory
        //    {
        //        HostName = _settings.HostName,
        //        UserName = _settings.Username,
        //        Password = _settings.Password
        //    };

        //}


        //// בנאי שמקבל הגדרות מהקובץ appsettings.json
        ////public RabbitMqService(IConfiguration configuration)
        ////{
        ////    _hostname = configuration["RabbitMq:HostName"] ?? "localhost";
        ////    _queueName = configuration["RabbitMq:QueueName"] ?? "item_queue";
        ////}

        //// הפונקציה ששולחת הודעה לתור
        //public Task PublishItemAsync(CreateItemRequest request)
        //{
        //    // 1. יצירת ConnectionFactory עם כתובת ה־RabbitMQ
        //    //var factory = new ConnectionFactory() { HostName = _hostname };
        //    var factory = new ConnectionFactory() { HostName = _settings.HostName };

        //    // 2. פתיחת חיבור ל־RabbitMQ
        //    using var connection = factory.CreateConnection();
        //    // 3. פתיחת Channel (ערוץ) דרך החיבור – ערוצים מאפשרים שליחה/קבלה של הודעות
        //    using var channel = connection.CreateModel();

        //    // 4. הגדרה של התור (אם לא קיים – נוצר עכשיו)
        //    channel.QueueDeclare(
        //        //queue: _queueName, // שם התור
        //        queue: _settings.ItemQueue,
        //        durable: true,// האם ההודעות נשמרות אחרי אתחול שרת (true = כן)
        //        exclusive: false,// האם רק חיבור זה יכול לגשת לתור (false = גישה משותפת)
        //        autoDelete: false, // האם התור נמחק אוטומטית כשהוא ריק (false = נשאר)
        //        arguments: null); // פרמטרים נוספים – אין כאן

        //    // 5. המרת האובייקט JSON למחרוזת ואז לבייטים
        //    var json = JsonSerializer.Serialize(request);// הופך את ה־DTO למחרוזת JSON
        //    var body = Encoding.UTF8.GetBytes(json); // הופך את המחרוזת לבייטים

        //    // 6. שליחת ההודעה לתור
        //    channel.BasicPublish(
        //        exchange: "",// שימוש ב־Default Exchange של RabbitMQ
        //        //routingKey: _queueName, // השם של התור
        //        routingKey: _settings.ItemQueue,
        //        basicProperties: null,// אין header מיוחד
        //        body: body);// גוף ההודעה – כ־byte[]

        //    return Task.CompletedTask;
        //}

        //public Task PublishUserAsync(CreateUserRequest request)
        //{
        //    var factory = new ConnectionFactory() { HostName = _settings.HostName };

        //    using var connection = factory.CreateConnection();
        //    using var channel = connection.CreateModel();

        //    channel.QueueDeclare(
        //        queue: _settings.UserQueue,
        //        durable: true,
        //        exclusive: false,
        //        autoDelete: false,
        //        arguments: null);

        //    var json = JsonSerializer.Serialize(request);
        //    var body = Encoding.UTF8.GetBytes(json);

        //    channel.BasicPublish(
        //        exchange: "",
        //        routingKey: _settings.UserQueue,
        //        basicProperties: null,
        //        body: body);

        //    return Task.CompletedTask;
        //}

        //public Task SendMessageAsync<T>(T message, string queueName)
        //{
        //    using var connection = _factory.CreateConnection();
        //    using var channel = connection.CreateModel();

        //    channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

        //    var json = JsonSerializer.Serialize(message);
        //    var body = Encoding.UTF8.GetBytes(json);

        //    var properties = channel.CreateBasicProperties();
        //    properties.Persistent = true;

        //    channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: properties, body: body);

        //    return Task.CompletedTask; // שימי לב לשורה הזו - חובה אם אין async
        //}


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
        public Task PublishItemAsync(CreateItemRequest request)
            => SendMessageAsync(request, _settings.ItemQueue);

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
