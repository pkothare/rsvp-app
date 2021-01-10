using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Opifex.Rsvp.Data
{
    public class RsvpConstraint
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public RsvpOptions OptionCombinations { get; set; }

        [Required]
        [Range(1, 10)]
        public int MaxGuests { get; set; }

        [ForeignKey(nameof(Id))]
        public ApplicationUser User { get; set; }
    }
}
