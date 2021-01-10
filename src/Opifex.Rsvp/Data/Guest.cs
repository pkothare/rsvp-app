using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Opifex.Rsvp.Data
{
    public class Guest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid RsvpId { get; set; }

        [ForeignKey(nameof(RsvpId))]
        public Rsvp Rsvp { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(250)]
        public string Name { get; set; }

    }
}
