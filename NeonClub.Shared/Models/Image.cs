using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeonClub.Shared.Models
{
    public class Image
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [StringLength(255)]
        public string OriginalName { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Path { get; set; } = string.Empty;

        public string MimeType { get; set; } = string.Empty;
        public long SizeBytes { get; set; }

        [Required]
        public DateTime UploadedAt { get; private set; } = DateTime.UtcNow;

        public Guid? ZoneId { get; set; }
        public Zone? Zone { get; set; }
    }
}
