using System.ComponentModel.DataAnnotations;

namespace Opifex.Rsvp.Data
{
    public class ZoomGuest : Guest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
