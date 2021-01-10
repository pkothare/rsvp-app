using Microsoft.AspNetCore.Identity;
using System;

namespace Opifex.Rsvp.Data
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public string Description { get; set; }
    }
}
