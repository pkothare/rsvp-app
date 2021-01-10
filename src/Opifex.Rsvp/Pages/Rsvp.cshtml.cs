using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Opifex.Rsvp.Data;

namespace Opifex.Rsvp.Pages
{
    [Authorize]
    public class RsvpModel : PageModel
    {
        private readonly ApplicationDbContext Context;

        public RsvpModel(ApplicationDbContext context)
        {
            Context = context;
        }

        public RsvpOptions OptionCombinations
        {
            get
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var constraints = Context.RsvpConstraints
                    .FirstOrDefault(x => x.Id == userId);
                return constraints.OptionCombinations;
            }
        }

        [BindProperty]
        public RsvpOptions ChosenOption { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                if ((OptionCombinations & ChosenOption) != ChosenOption)
                {
                    ModelState.AddModelError(nameof(ChosenOption), "Option not allowed.");
                    return Page();
                }
                else
                {
                    var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    var rsvp = await Context.Rsvps.FindAsync(userId);
                    if (rsvp != null) // Found existing RSVP
                    {
                        if (rsvp.ChosenOption == ChosenOption) // They chose the same option
                        {
                            return RedirectToPage("Dashboard");
                        }
                        else // They chose a new option
                        {
                            Context.Rsvps.Remove(rsvp);
                            await Context.SaveChangesAsync();
                        }
                    }
                    // Either the old RSVP was deleted or this is brand new
                    Context.Rsvps.Add(new Data.Rsvp
                    {
                        Id = userId,
                        ChosenOption = ChosenOption,
                    });
                    await Context.SaveChangesAsync();
                    return RedirectToPage("Dashboard");
                }
            }
            else
            {
                ModelState.AddModelError(nameof(ChosenOption), "There was an error in updating your RSVP");
                return Page();
            }
        }
    }
}
