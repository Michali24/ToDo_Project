using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDo.Core.DTOs
{
    public class CreateItemRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public required string Title { get; set; }
        [StringLength(300)]
        public string? Description { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public required int UserId { get; set; }
    }
}
