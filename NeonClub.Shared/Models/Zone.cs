using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeonClub.Shared.Models
{
    public class Zone
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(50)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Subtitle { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;

        [Required]
        [Precision(10, 2)]
        public decimal Price { get; set; }

        [Required]
        [StringLength(10)]
        public string Emoji { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Tag { get; set; }

        [Required]
        public bool Popular { get; set; } = false;

        [Required]
        [StringLength(100)]
        public string Cpu { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Gpu { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Ram { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Monitor { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Description { get; set; } = string.Empty;
        
        public List<string> Features { get; set; } = [];

        [Required]
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;


        public ICollection<Booking> Bookings { get; set; } = [];
    }
}
