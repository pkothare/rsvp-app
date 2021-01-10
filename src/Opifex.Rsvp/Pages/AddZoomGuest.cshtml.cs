using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Opifex.Rsvp.Data;

namespace Opifex.Rsvp.Pages
{
    [Authorize]
    public class AddZoomGuestModel : PageModel
    {
        private readonly Opifex.Rsvp.Data.ApplicationDbContext Context;

        public AddZoomGuestModel(ApplicationDbContext context)
        {
            Context = context;
        }

        public Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        public Data.Rsvp Rsvp => Context.Rsvps
            .FirstOrDefault(x => x.Id == UserId);

        public RsvpOptions ChosenOption => Rsvp.ChosenOption;

        public IActionResult OnGet()
        {
            if(Rsvp.ChosenOption == RsvpOptions.Zoom)
            {
                return Page();
            }
            return RedirectToPage("./Dashboard");
        }

        [BindProperty]
        public ZoomGuest ZoomGuest { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            else if(ChosenOption == RsvpOptions.Zoom)
            {
                ZoomGuest.RsvpId = UserId;
                Context.ZoomGuests.Add(ZoomGuest);
                await Context.SaveChangesAsync();
            }

            return RedirectToPage("./Dashboard");
        }
    }
}
