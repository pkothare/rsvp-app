using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Opifex.Rsvp.Data;

namespace Opifex.Rsvp.Pages
{
    [Authorize]
    public class DashboardModel : PageModel
    {
        private readonly ApplicationDbContext Context;

        public DashboardModel(ApplicationDbContext context)
        {
            Context = context;
        }

        public Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        public Data.Rsvp Rsvp => Context.Rsvps
            .FirstOrDefault(x => x.Id == UserId);

        public RsvpOptions ChosenOption => Rsvp.ChosenOption;

        public IEnumerable<Guest> Guests => Context.Guests
            .Where(x => x.RsvpId == Rsvp.Id);

        public int MaxGuests => Context.RsvpConstraints
            .First(x => x.Id == UserId)
            .MaxGuests;

        public IActionResult OnGet()
        {
            if(Rsvp == null)
            {
                return RedirectToPage("Rsvp");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostDownloadAsync(Guid guestId)
        {
            var foundGuest = Guests.FirstOrDefault(x => x.Id == guestId);
            if (foundGuest != null)
            {
                var inPersonGuest = await Context.InPersonGuests.FindAsync(guestId);
                return File(inPersonGuest.Content, inPersonGuest.ContentType, @$"{guestId}{inPersonGuest.Extension}");
            }
            else
            {
                ModelState.AddModelError(nameof(Guests), "No file was found for the guest.");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostRemoveAsync(Guid guestId)
        {
            var foundGuest = Guests.FirstOrDefault(x => x.Id == guestId);
            if(foundGuest != null)
            {
                Context.Guests.Remove(foundGuest);
                await Context.SaveChangesAsync();
            }
            else
            {
                ModelState.AddModelError(nameof(Guests), "The guest you were trying to remove was not found.");
            }
            return Page();
        }
    }
}
