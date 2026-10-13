using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeonClub.Shared.Models
{
    [Table(nameof(Booking))]
    public class Booking
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(Zone))]
        public Guid ZoneId { get; set; }
        public Zone Zone { get; set; } = null!;

        [Required]
        public int Hours { get; set; } = 1;

        [Required]
        public decimal Total { get; set; } = 0;

        [Required]
        [ForeignKey(nameof(Status))]
        public int StatusId { get; set; }
        public Status Status { get; set; } = null!;

        [Required]
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    }
}
