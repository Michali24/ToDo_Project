using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ToDo.Core.DTOs;
using ToDo.Core.Interfaces.Services;
using ToDo.Core.Settings;
using Microsoft.Extensions.Logging;


namespace ToDo.Service.Services
{
    public class RabbitMqRpc:IRabbitMqRpc,IDisposable
    {
        ////DI
        //private readonly IConnection _conn;//TCP connection to the broker
        //private readonly IModel _pubCh;//Channel for requests
        //private readonly IModel _replyCh;//Response channel from the consumer (reply queue)
        //private readonly string _replyQueue;
        //private readonly ConcurrentDictionary<string, TaskCompletionSource<byte[]>> _pending = new();//Dictionary with key=CorrelationId, value=TaskCompletionSource (byte[])

        //public RabbitMqRpc(IOptions<RabbitMqSettings> opt)
        //{
        //    var s = opt.Value;
        //    var factory = new ConnectionFactory
        //    {
        //        HostName = s.HostName,
        //        UserName = s.Username,
        //        Password = s.Password,
        //        DispatchConsumersAsync = true // חשוב כדי לעבוד עם AsyncEventingBasicConsumer
        //    };

        //    //An open TCP connection between your app and the broker (RabbitMQ server).
        //    _conn = factory.CreateConnection();
        //    _pubCh = _conn.CreateModel();//Channel to request
        //    _replyCh = _conn.CreateModel();//Channel to reply

        //    //Temporary queue
        //    var q = _replyCh.QueueDeclare(
        //        queue: $"rpc_reply_queue_{Guid.NewGuid():N}",
        //        durable: false,
        //        exclusive: true,
        //        autoDelete: true,
        //        arguments: null
        //    );
        //    _replyQueue = q.QueueName;

        //    var consumer = new AsyncEventingBasicConsumer(_replyCh);//Create a consumer that listens to the reply queue
        //    consumer.Received += async (_, ea) =>
        //    {
        //        var corr = ea.BasicProperties?.CorrelationId;//The CorrelationId of the queue taken from the message properties
        //        if (corr != null && _pending.TryRemove(corr, out var tcs))
        //        {
        //            tcs.TrySetResult(ea.Body.ToArray());
        //        }
        //        _replyCh.BasicAck(ea.DeliveryTag, false);
        //        await Task.CompletedTask;
        //    };

        //    _replyCh.BasicConsume(_replyQueue,
        //                          autoAck: false, 
        //                          consumer: consumer);
        //}

        //public async Task<TResponse> RequestAsync<TRequest, TResponse>(
        //    string requestQueue,
        //    TRequest payload,//
        //    TimeSpan timeout,//
        //    CancellationToken ct = default)//
        //{
        //    var corr = Guid.NewGuid().ToString("N");//Create unique CorrelationID
        //    var tcs = new TaskCompletionSource<byte[]>(TaskCreationOptions.RunContinuationsAsynchronously);//
        //    if (!_pending.TryAdd(corr, tcs))
        //        throw new InvalidOperationException("Correlation collision.");

        //    var props = _pubCh.CreateBasicProperties();
        //    props.CorrelationId = corr;
        //    props.ReplyTo = _replyQueue;
        //    props.ContentType = "application/json";
        //    props.Persistent = true; // לשמירת הודעה עם durable queue

        //    var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload));
        //    _pubCh.BasicPublish(exchange: "", routingKey: requestQueue, basicProperties: props, body: body);

        //    using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        //    cts.CancelAfter(timeout);
        //    await using var _ = cts.Token.Register(() =>
        //    {
        //        if (_pending.TryRemove(corr, out var waiting))
        //            waiting.TrySetException(new TimeoutException("RPC timeout"));
        //    });

        //    var respBytes = await tcs.Task; // ייזרק TimeoutException אם נגמר הזמן
        //    return JsonSerializer.Deserialize<TResponse>(respBytes)!;
        //}

        //public void Dispose()
        //{
        //    _replyCh?.Close();
        //    _pubCh?.Close();
        //    _conn?.Close();
        //}

        //---------------------------------------------------------
        //private readonly ILogger<RabbitMqRpc> _logger;   // ✅ הוספת Logger

        //private readonly RabbitMqSettings _settings;                  // הגדרות: שמות תורים, פרטי התחברות
        //private readonly IConnection _conn;                           // חיבור TCP בודד ל-broker (יעיל יותר מפתיחה/סגירה חוזרת)
        //private readonly IModel _pubCh;                               // ערוץ לפרסום בקשות RPC (publish של request)
        //private readonly IModel _replyCh;                             // ערוץ לצריכת תשובות (reply queue consumer)
        //private readonly string _replyQueue;                          // שם התור הזמני לתשובות (ReplyTo)
        //private readonly ConcurrentDictionary<string,                 // מיפוי CorrelationId -> TCS<byte[]> כדי להשלים את הבקשה המתאימה
        //    TaskCompletionSource<byte[]>> _pending = new();

        //private readonly object _pubSync = new();                     // 🔒 נעילה: IModel אינו Thread-Safe → ננעל סביב פעולות על _pubCh

        //public RabbitMqRpc(IOptions<RabbitMqSettings> opt, ILogger<RabbitMqRpc> logger)            // DI: קבלת ההגדרות מה־configuration
        //{
        //    _logger = logger;
        //    _settings = opt.Value;                                    // שמירת ההגדרות לשימוש פנימי במחלקה

        //    var factory = new ConnectionFactory                       // בניית מפעל חיבורים RabbitMQ עם פרטי ההתחברות
        //    {
        //        HostName = _settings.HostName,                        // כתובת שרת ה-RabbitMQ
        //        UserName = _settings.Username,                        // שם משתמש
        //        Password = _settings.Password,                        // סיסמה
        //        DispatchConsumersAsync = true                         // חשוב: מאפשר ל-consumer לעבוד עם async handlers
        //    };

        //    _conn = factory.CreateConnection();                        // פתיחת חיבור TCP אחד (מונע overhead של פתיחה/סגירה לכל בקשה)
        //    _pubCh = _conn.CreateModel();                              // יצירת ערוץ פרסום (publish) עבור בקשות RPC
        //    _replyCh = _conn.CreateModel();                            // יצירת ערוץ צריכה (consume) עבור תשובות RPC

        //    // יצירת תור תשובות זמני ייחודי לאינסטנס הזה (ReplyTo):
        //    // durable:false – לא צריך לשרוד ריסט; exclusive:true – בלעדי לחיבור הזה; autoDelete:true – יימחק כשהחיבור ייסגר
        //    var q = _replyCh.QueueDeclare(
        //        queue: $"rpc_reply_queue_{Guid.NewGuid():N}",          // שם ייחודי עם GUID (מונע התנגשויות בין אינסטנסים)
        //        durable: false,
        //        exclusive: true,
        //        autoDelete: true,
        //        arguments: null);
        //    _replyQueue = q.QueueName;                                 // נשמור את שם התור כדי לשים אותו ב-ReplyTo לכל בקשה

        //    // צרכן אסינכרוני שיקבל את התשובות מה-Worker (שנשלחות ל-reply queue שלנו)
        //    var consumer = new AsyncEventingBasicConsumer(_replyCh);   // consumer שתומך ב-async
        //    consumer.Received += async (_, ea) =>                      // אירוע: התקבלה הודעה (תשובה) בתור ה-reply
        //    {
        //        var corr = ea.BasicProperties?.CorrelationId;          // מזהה איזה בקשה זו (מגיע מהבקשה המקורית)
        //        if (corr != null && _pending.TryRemove(corr, out var tcs)) // מוצאים את ה-TCS שמחכה לתשובה עבור corr הזה
        //        {
        //            tcs.TrySetResult(ea.Body.ToArray());               // משחררים את ההמתנה ומעבירים את גוף התשובה (byte[])
        //        }
        //        _replyCh.BasicAck(ea.DeliveryTag, multiple: false);    // ack ידני – מסירים את ההודעה מהתור בהצלחה
        //        await Task.CompletedTask;                              // אין עוד עבודה כאן – משלים את ה-handler

        //        _logger?.LogInformation("📩 Received RPC reply (CorrelationId={CorrelationId}): {Body}",
        //      corr, Encoding.UTF8.GetString(ea.Body.ToArray()));
        //    };




        //    _replyCh.BasicConsume(                                     // התחלת צריכה מהתור הזמני של תשובות
        //        queue: _replyQueue,                                    // מאיזה תור לצרוך – התור הזמני הייחודי שלנו
        //        autoAck: false,                                        // ניהול ack ידני (כמו שעשינו ב-Received)
        //        consumer: consumer);                                   // אותו consumer שבנינו
        //}

        ///// <summary>
        ///// ה-"Core": שליחת בקשת RPC לתור נתון (requestQueue), ממתין עד לתשובה או Timeout
        ///// </summary>
        //public async Task<TResponse> RequestAsync<TRequest, TResponse>( // Generic: סוג בקשה ותשובה
        //    string requestQueue,                                        // לאיזה תור לשלוח (ItemQueue/UserQueue)
        //    TRequest payload,                                           // גוף הבקשה (יהפוך ל-JSON)
        //    TimeSpan? timeout = null,                                   // ברירת מחדל: 30 שניות (אם לא צוין)
        //    CancellationToken ct = default)                             // ביטול חיצוני (ביטול בקשת HTTP למשל)
        //{
        //    EnsureQueueExists(requestQueue);                            // ודואגים שהתור היעד קיים (Idempotent ובטוח)

        //    var corr = Guid.NewGuid().ToString("N");                    // CorrelationId ייחודי לזיהוי השיוך בין בקשה לתשובה
        //    var tcs = new TaskCompletionSource<byte[]>(
        //        TaskCreationOptions.RunContinuationsAsynchronously);    // TCS שיתמלא כשנקלוט את התשובה

        //    if (!_pending.TryAdd(corr, tcs))                            // רושמים את ה-corr במפה; הגנה נדירה על התנגשות
        //        throw new InvalidOperationException("Correlation collision – duplicate ID.");

        //    IBasicProperties props;                                      // ניצור properties עם ReplyTo+CorrelationId בצורה thread-safe
        //    byte[] body;                                                // וגם נכין את ה-body לשליחה

        //    lock (_pubSync)                                             // 🔒 IModel אינו Thread-Safe → ננעל בעת שימוש ב-_pubCh
        //    {
        //        props = _pubCh.CreateBasicProperties();                 // בניית Properties להודעה (מטא-דאטה)
        //        props.CorrelationId = corr;                             // חובה ב-RPC: לזהות את התשובה המתאימה
        //        props.ReplyTo = _replyQueue;                            // חובה ב-RPC: לאן להחזיר את התשובה
        //        props.ContentType = "application/json";                 // מציין JSON
        //        props.Persistent = true;                                // בקשה לשמירה בדיסק (אם התורים durable)

        //        body = Encoding.UTF8.GetBytes(                          // Serialize → bytes
        //            JsonSerializer.Serialize(payload));

        //        _pubCh.BasicPublish(                                    // פרסום הבקשה לתור שה-Worker צורך ממנו
        //            exchange: "",                                       // exchange ברירת מחדל (direct)
        //            routingKey: requestQueue,                           // שם התור היעד (Item/User)
        //            basicProperties: props,                             // כולל ReplyTo + CorrelationId
        //            body: body);                                        // גוף הבקשה
        //    }

        //    using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct); // מאחדים ביטול חיצוני עם Timeout פנימי
        //    cts.CancelAfter(timeout ?? TimeSpan.FromSeconds(30));                // ברירת מחדל: 30 שניות

        //    using var reg = cts.Token.Register(() =>                               // אם בוטל/פג הזמן – נשחרר את הממתין בשגיאה
        //    {
        //        if (_pending.TryRemove(corr, out var waiting))                     // מסירים את ה-corr מהמפה
        //            waiting.TrySetException(new TimeoutException(                  // ממלאים את ה-Task בשגיאת Timeout ברורה
        //                $"RPC timed out after {(timeout ?? TimeSpan.FromSeconds(30)).TotalSeconds} seconds."));
        //    });

        //    var respBytes = await tcs.Task.ConfigureAwait(false);                   // ממתינים לתשובה (ה-consumer ישלים את ה-TCS)
        //    return JsonSerializer.Deserialize<TResponse>(respBytes)!;               // ממירים את ה-byte[] שקיבלנו לאובייקט תגובה ומחזירים
        //}

        ///// <summary>
        ///// RPC ייעודי ל-UserQueue: לא צריך לזכור את שם התור בקוד הקורא
        ///// </summary>
        //public Task<TResponse> RequestUserAsync<TRequest, TResponse>(              // עוטף את ה-Core עם שם התור של Users
        //    TRequest payload,
        //    TimeSpan? timeout = null,
        //    CancellationToken ct = default)
        //    => RequestAsync<TRequest, TResponse>(_settings.UserQueue, payload, timeout, ct);

        ///// <summary>
        ///// RPC ייעודי ל-ItemQueue: לא צריך לזכור את שם התור בקוד הקורא
        ///// </summary>
        //public Task<TResponse> RequestItemAsync<TRequest, TResponse>(              // עוטף את ה-Core עם שם התור של Items
        //    TRequest payload,
        //    TimeSpan? timeout = null,
        //    CancellationToken ct = default)
        //    => RequestAsync<TRequest, TResponse>(_settings.ItemQueue, payload, timeout, ct);

        //private void EnsureQueueExists(string queueName)                            // פונקציית עזר: ודא שהתור קיים (Idempotent)
        //{
        //    lock (_pubSync)                                                        // 🔒 גם QueueDeclare נעשה על אותו ערוץ → ננעל
        //    {
        //        _pubCh.QueueDeclare(                                               // אם התור קיים – לא ייפגע; אם לא – יווצר
        //            queue: queueName,
        //            durable: true,                                                 // שורד ריסט (יחד עם Persistent על הודעות)
        //            exclusive: false,                                              // פתוח לחיבורים אחרים (ה-Worker)
        //            autoDelete: false,                                             // לא נמחק אוטומטית
        //            arguments: null);
        //    }
        //}

        //public void Dispose()                                                       // ניקוי מסודר של משאבים
        //{
        //    try { _replyCh?.Close(); } catch { /* ignore */ }                      // סוגר ערוץ תשובות
        //    try { _pubCh?.Close(); } catch { /* ignore */ }                        // סוגר ערוץ פרסום
        //    try { _conn?.Close(); } catch { /* ignore */ }                         // סוגר חיבור TCP
        //}


        ///----================================================
        private readonly ILogger<RabbitMqRpc> _logger;   // ✅ Logger להדפסה ל-Output
        private readonly RabbitMqSettings _settings;
        private readonly IConnection _conn;
        private readonly IModel _pubCh;
        private readonly IModel _replyCh;
        private readonly string _replyQueue;
        private readonly ConcurrentDictionary<string, TaskCompletionSource<byte[]>> _pending = new();
        private readonly object _pubSync = new();

        public RabbitMqRpc(IOptions<RabbitMqSettings> opt, ILogger<RabbitMqRpc> logger)
        {
            _logger = logger;
            _settings = opt.Value;

            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                UserName = _settings.Username,
                Password = _settings.Password,
                DispatchConsumersAsync = true
            };

            _logger.LogInformation("🔗 Creating RabbitMQ connection to {HostName}...", _settings.HostName);
            _conn = factory.CreateConnection();
            _pubCh = _conn.CreateModel();
            _replyCh = _conn.CreateModel();

            var q = _replyCh.QueueDeclare(
                queue: $"rpc_reply_queue_{Guid.NewGuid():N}",
                durable: false,
                exclusive: true,
                autoDelete: true,
                arguments: null);

            _replyQueue = q.QueueName;
            _logger.LogInformation("📩 Declared temporary reply queue: {QueueName}", _replyQueue);

            var consumer = new AsyncEventingBasicConsumer(_replyCh);
            consumer.Received += async (_, ea) =>
            {
                var corr = ea.BasicProperties?.CorrelationId;
                _logger.LogInformation("📥 Received reply from Worker (CorrelationId={CorrelationId})", corr);

                if (corr != null && _pending.TryRemove(corr, out var tcs))
                {
                    tcs.TrySetResult(ea.Body.ToArray());
                }
                _replyCh.BasicAck(ea.DeliveryTag, multiple: false);

                _logger.LogInformation("✅ Reply acknowledged for CorrelationId={CorrelationId}", corr);
                await Task.CompletedTask;
            };

            _replyCh.BasicConsume(
                queue: _replyQueue,
                autoAck: false,
                consumer: consumer);

            _logger.LogInformation("✅ Listening for replies on reply queue: {QueueName}", _replyQueue);
        }

        public async Task<TResponse> RequestAsync<TRequest, TResponse>(
            string requestQueue,
            TRequest payload,
            TimeSpan? timeout = null,
            CancellationToken ct = default)
        {
            EnsureQueueExists(requestQueue);

            var corr = Guid.NewGuid().ToString("N");
            var tcs = new TaskCompletionSource<byte[]>(TaskCreationOptions.RunContinuationsAsynchronously);

            if (!_pending.TryAdd(corr, tcs))
                throw new InvalidOperationException("Correlation collision – duplicate ID.");

            IBasicProperties props;
            byte[] body;

            lock (_pubSync)
            {
                props = _pubCh.CreateBasicProperties();
                props.CorrelationId = corr;
                props.ReplyTo = _replyQueue;
                props.ContentType = "application/json";
                props.Persistent = true;

                body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload));

                _logger.LogInformation("📤 Publishing message to {Queue} (CorrelationId={CorrelationId})", requestQueue, corr);
                _pubCh.BasicPublish(
                    exchange: "",
                    routingKey: requestQueue,
                    basicProperties: props,
                    body: body);
                _logger.LogInformation("✅ Message published successfully to {Queue}", requestQueue);
            }

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(timeout ?? TimeSpan.FromSeconds(30));

            using var reg = cts.Token.Register(() =>
            {
                if (_pending.TryRemove(corr, out var waiting))
                {
                    _logger.LogWarning("⏱️ RPC request to {Queue} timed out after {Seconds} seconds (CorrelationId={CorrelationId})",
                        requestQueue, (timeout ?? TimeSpan.FromSeconds(30)).TotalSeconds, corr);
                    waiting.TrySetException(new TimeoutException(
                        $"RPC timed out after {(timeout ?? TimeSpan.FromSeconds(30)).TotalSeconds} seconds."));
                }
            });

            var respBytes = await tcs.Task.ConfigureAwait(false);
            _logger.LogInformation("📦 Deserializing RPC response for CorrelationId={CorrelationId}", corr);

            return JsonSerializer.Deserialize<TResponse>(respBytes)!;
        }

        public Task<TResponse> RequestUserAsync<TRequest, TResponse>(
            TRequest payload,
            TimeSpan? timeout = null,
            CancellationToken ct = default)
            => RequestAsync<TRequest, TResponse>(_settings.UserQueue, payload, timeout, ct);

        public Task<TResponse> RequestItemAsync<TRequest, TResponse>(
            TRequest payload,
            TimeSpan? timeout = null,
            CancellationToken ct = default)
            => RequestAsync<TRequest, TResponse>(_settings.ItemQueue, payload, timeout, ct);

        private void EnsureQueueExists(string queueName)
        {
            lock (_pubSync)
            {
                _pubCh.QueueDeclare(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
            }
        }

        public void Dispose()
        {
            try { _replyCh?.Close(); } catch { }
            try { _pubCh?.Close(); } catch { }
            try { _conn?.Close(); } catch { }
        }
    }
}
