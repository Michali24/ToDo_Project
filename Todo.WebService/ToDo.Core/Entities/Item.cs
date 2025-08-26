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
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public bool IsCompleted { get; set; } = false;
        public bool IsDeleted { get; set; } = false; // ✅ soft delete


        // Foreign key
        [ForeignKey("UserId")]
        public int UserId { get; set; }

        // Navigation property
        public virtual User User { get; set; } = null!;
    }
}
