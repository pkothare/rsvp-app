using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Opifex.Rsvp.Data
{
    public class Rsvp : IValidatableObject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public RsvpOptions ChosenOption { get; set; }

        [ForeignKey(nameof(Id))]
        public ApplicationUser User { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if ((int)ChosenOption > 2)
            {
                yield return new ValidationResult("Only one RsvpOption is allowed.",
                    new[] { nameof(ChosenOption) });
            }
        }
    }
}
