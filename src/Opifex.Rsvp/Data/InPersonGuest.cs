using System.ComponentModel.DataAnnotations;

namespace Opifex.Rsvp.Data
{
    public class InPersonGuest : Guest
    {
        [Required]
        [MaxLength(10485760)] // 10 MB
        public byte[] Content { get; set; }

        [Required]
        public string Extension { get; set; }

        [Required]
        public string ContentType { get; set; }
    }
}
