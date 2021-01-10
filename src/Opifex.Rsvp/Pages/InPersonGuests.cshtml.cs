using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Opifex.Rsvp.Data;

namespace Opifex.Rsvp.Pages
{
    [Authorize(Roles = "Administrator,Manager")]
    public class InPersonGuestsModel : PageModel
    {
        private ApplicationDbContext Context;
        private IEmailSender EmailSender;


        public InPersonGuestsModel(ApplicationDbContext context, IEmailSender emailSender)
        {
            Context = context;
            EmailSender = emailSender;
        }

        public IQueryable<InPersonGuest> Rows => Context.InPersonGuests.AsQueryable();

        public IActionResult OnGetAsync()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostDownloadAsync(Guid guestId)
        {
            var guest = await Context.InPersonGuests.FirstOrDefaultAsync(x => x.Id == guestId);
            if (guest != null)
            {
                return File(guest.Content, guest.ContentType, @$"{guestId}{guest.Extension}");
            }
            else
            {
                ModelState.AddModelError(nameof(Rows), "No file was found for the guest.");
            }
            return Page();
        }
    }
}
