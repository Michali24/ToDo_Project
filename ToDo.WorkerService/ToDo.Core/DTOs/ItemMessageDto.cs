using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDo.Core.DTOs
{
    public class ItemMessageDto
    {
        public int ItemId { get; set; }         // חובה ל־Complete/Delete
        public string? Title { get; set; }      // חובה ל־Create
        public int UserId { get; set; }         // חובה ל־Create
        public string Description { get; set; } = string.Empty;

        public string Action { get; set; } // "Create", "Complete", "Delete"

    }
}
