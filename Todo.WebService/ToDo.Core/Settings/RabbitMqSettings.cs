using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDo.Core.Settings
{
    public class RabbitMqSettings
    {
        public string HostName { get; set; } = "localhost";
        public string Username { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string ItemQueue { get; set; } = "item_queue";
        public string UserQueue { get; set; } = "user_queue";

    }
}
