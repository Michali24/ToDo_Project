using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDo.Core.Entities
{
    public class Item
    {
        [Key]
        public int Id { get; set; }  // מפתח ראשי

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;
        public bool IsCompleted { get; set; } = false;
        public bool IsDeleted { get; set; } = false;  // מחיקה רכה (soft delete)

        // קשר למשתמש
        [ForeignKey("UserId")]
        public int UserId { get; set; }
        public virtual User User { get; set; } = null!;
    }
}

