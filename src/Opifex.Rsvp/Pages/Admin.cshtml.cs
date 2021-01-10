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
    public class AdminModel : PageModel
    {
        private ApplicationDbContext Context;
        private IEmailSender EmailSender;

        private static EmailTemplate _inPersonTemplate = new EmailTemplate("wwwroot\\email-welcome-inperson.html");
        private static EmailTemplate _zoomTemplate = new EmailTemplate("wwwroot\\email-welcome-zoom.html");

        public class RsvpDataModel
        {
            public Guid UserId { get; set; }
            public string Name { get; set; }
            public string EmailAddress { get; set; }
            public int MaxGuests { get; set; }
            public string RsvpType { get; set; }
            public bool HasRsvped { get; set; }
            public string ChosenRsvp { get; set; }
            public int NumberOfGuestsAdded { get; set; }
        }

        public AdminModel(ApplicationDbContext context, IEmailSender emailSender)
        {
            Context = context;
            EmailSender = emailSender;
        }

        public IList<RsvpDataModel> RsvpData { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var data = new List<RsvpDataModel>();
            var users = Context.Users;
            foreach (var user in users)
            {
                var constraint = await Context.RsvpConstraints.FindAsync(user.Id);
                var rsvp = await Context.Rsvps.FirstOrDefaultAsync(x => x.Id == user.Id);
                var guestCount = Context.Guests.Count(x => x.RsvpId == user.Id);

                var datum = new RsvpDataModel
                {
                    UserId = user.Id,
                    Name = user.DisplayName,
                    EmailAddress = user.Email,
                    MaxGuests = constraint.MaxGuests,
                    RsvpType = constraint.OptionCombinations.ToString(),
                    HasRsvped = rsvp != null,
                    ChosenRsvp = rsvp?.ChosenOption.ToString(),
                    NumberOfGuestsAdded = guestCount
                };

                data.Add(datum);
            }
            RsvpData = data;
            return Page();
        }

        public async Task<IActionResult> OnPostSendWelcomeEmailAsync(Guid userId)
        {
            var foundUser = await Context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            var constraint = await Context.RsvpConstraints.FirstOrDefaultAsync(x => x.Id == userId);

            if (foundUser != null && constraint != null)
            {
                EmailTemplate template = null;
                if ((constraint.OptionCombinations & RsvpOptions.InPerson) == RsvpOptions.InPerson)
                {
                    template = _inPersonTemplate;
                }
                else if ((constraint.OptionCombinations & RsvpOptions.Zoom) == RsvpOptions.Zoom)
                {
                    template = _zoomTemplate;
                }

                var email = template.MergeWith(foundUser.DisplayName);

                await EmailSender.SendEmailAsync(
                 foundUser.Email,
                 "You have been invited to Durwa and Pranav's Marriage Celebration", email);
            }
            return RedirectToAction("./Admin");
        }
    }
}
