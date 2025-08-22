using System;
using System.ComponentModel.DataAnnotations;

namespace ContentService.Domain.Entities
{
    public class Content
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Title { get; set; }
        [Required]
        [StringLength(500)]
        public string Body { get; set; }
        [Required]
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
