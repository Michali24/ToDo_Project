using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDo.Core.DTOs
{
    public class ItemMessageDto
    {
        [Range(1, int.MaxValue)]
        public int ItemId { get; set; }         // חובה ל־Complete/Delete
        
        [StringLength(100, MinimumLength = 3)]
        public string? Title { get; set; }      // חובה ל־Create
        
        public int UserId { get; set; }         // חובה ל־Create
       
        [StringLength(300)]
        public string? Description { get; set; }
     
        [Required]
        public required string Action { get; set; }  // "Create", "Complete", "Delete"
    }
}
