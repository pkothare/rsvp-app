using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Opifex.Rsvp.Data;
using System;
using System.Linq;
using System.Security.Claims;

namespace Opifex.Rsvp.Pages
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext Context;

        public DetailsModel(ApplicationDbContext context)
        {
            Context = context;
        }

        public Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        public Data.Rsvp Rsvp => Context.Rsvps
            .FirstOrDefault(x => x.Id == UserId);

    }
}
