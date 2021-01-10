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
    public class ZoomGuestsModel : PageModel
    {
        private ApplicationDbContext Context;
        private IEmailSender EmailSender;

        private static EmailTemplate _zoomInviteTemplate = new EmailTemplate("wwwroot\\email-zoom-invite.html");



        public ZoomGuestsModel(ApplicationDbContext context, IEmailSender emailSender)
        {
            Context = context;
            EmailSender = emailSender;
        }

        public IQueryable<ZoomGuest> Rows => Context.ZoomGuests.AsQueryable();

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostSendZoomInviteAsync(Guid guestId)
        {
            var guest = await Context.ZoomGuests.FindAsync(guestId);

            if (guest != null)
            {

                var email = _zoomInviteTemplate.Template;

                await EmailSender.SendEmailAsync(
                 guest.Email,
                 "Durwa and Pranav's Marriage Celebration Zoom Invite", email);
            }
            return RedirectToAction("./ZoomGuests");
        }
    }
}
