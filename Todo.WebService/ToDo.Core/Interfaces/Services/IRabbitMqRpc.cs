using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo.Core.DTOs;

namespace ToDo.Core.Interfaces.Services
{
    public interface IRabbitMqRpc
    {
        //Task<TResponse> RequestAsync<TRequest, TResponse>(
        //  string requestQueue,
        //  TRequest payload,
        //  TimeSpan timeout,
        //  CancellationToken ct = default);



        //-----------------------------------------------

        //// שליחה חד-כיוונית (כמו RabbitMqService.SendMessageAsync)
        //Task PublishAsync<T>(string queueName, T message);

        //// sugar methods שימוש נוח לתורים הספציפיים שלך (Item/User)
        //Task PublishItemAsync(ItemMessageDto message);
        //Task PublishUserAsync(CreateUserRequest request);

        //// שליחת בקשה וחזרה עם תשובה (RPC)
        //Task<TResponse> RequestAsync<TRequest, TResponse>(
        //    string requestQueue,
        //    TRequest payload,
        //    TimeSpan timeout,
        //    CancellationToken ct = default);

        //----------------------------------------------------

        // שליחת RPC כללית: שם תור, payload, timeout אופציונלי, וביטול
        Task<TResponse> RequestAsync<TRequest, TResponse>(            // מחזיר TResponse אחרי שהתשובה מה-Worker התקבלה
            string requestQueue,                                      // לאיזה תור לשלוח (Item/User וכד')
            TRequest payload,                                         // גוף הבקשה
            TimeSpan? timeout = null,                                 // ברירת מחדל נקבעת במימוש (לרוב 30ש')
            CancellationToken ct = default);                          // ביטול חיצוני (למשל אם בקשת HTTP בוטלה)

        // sugar: RPC ייעודי לתור המשתמשים (מסתיר את שם התור)
        Task<TResponse> RequestUserAsync<TRequest, TResponse>(        // פשוט עוטף את RequestAsync עם UserQueue
            TRequest payload,
            TimeSpan? timeout = null,
            CancellationToken ct = default);

        // sugar: RPC ייעודי לתור הפריטים (מסתיר את שם התור)
        Task<TResponse> RequestItemAsync<TRequest, TResponse>(        // פשוט עוטף את RequestAsync עם ItemQueue
            TRequest payload,
            TimeSpan? timeout = null,
            CancellationToken ct = default);

    }
}
