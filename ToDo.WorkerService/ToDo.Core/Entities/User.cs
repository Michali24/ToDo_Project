using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDo.Core.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }  // מפתח ראשי

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Password { get; set; } = string.Empty;

        // קשר של אחד-לרבים מול Items
        public ICollection<Item> Items { get; set; } = new List<Item>();
    }
}
