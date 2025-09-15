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
        public int Id { get; set; } 

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public required string Title { get; set; }

        [StringLength(300)]
        public string? Description { get; set; } = string.Empty;

        public bool IsCompleted { get; set; } = false;
        public bool IsDeleted { get; set; } = false;  // ✅ soft delete

        // Foreign key
        [ForeignKey("UserId")]
        public int UserId { get; set; }

        // Navigation property
        public virtual User User { get; set; } = null!;
    }
}

