using Microsoft.AspNetCore.Identity;
using System;

namespace Opifex.Rsvp.Data
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        [PersonalData]
        public string DisplayName { get; set; }
    }
}
