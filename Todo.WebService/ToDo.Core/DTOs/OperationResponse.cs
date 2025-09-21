using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDo.Core.DTOs
{
    public class OperationResponse
    {
        public int Id { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public string? Message { get; set; }
        public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;
    }
}
