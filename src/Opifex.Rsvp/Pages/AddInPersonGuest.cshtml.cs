using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Opifex.Rsvp.Data;

namespace Opifex.Rsvp.Pages
{
    [Authorize]
    public class AddInPersonGuestModel : PageModel
    {
        private readonly ApplicationDbContext Context;

        public AddInPersonGuestModel(ApplicationDbContext context)
        {
            Context = context;
        }

        public Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        [BindProperty]
        public InPersonGuestInputModel InPersonGuest { get; set; }

        public class InPersonGuestInputModel : IValidatableObject
        {
            [Required]
            [MinLength(3)]
            [MaxLength(250)]
            public string Name { get; set; }

            [Required]
            public IFormFile Document { get; set; }

            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            {
                if (Document != null)
                {
                    if (Document.Length > 10485760)
                    {
                        yield return new ValidationResult("File upload size limit is 10 MB.", new[] { nameof(Document) });
                    }
                    string message = null;
                    try
                    {
                        AllowedFileExtensions.ValidateFile(Document);
                    }
                    catch (Exception e)
                    {
                        message = e.Message;
                    }
                    if (message != null)
                    {
                        yield return new ValidationResult(message, new[] { nameof(Document) });
                    }
                }
            }
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            using (var memoryStream = new MemoryStream())
            {
                await InPersonGuest.Document.CopyToAsync(memoryStream);
                var guest = new InPersonGuest
                {
                    Extension = AllowedFileExtensions.GetNormalizedExtension(InPersonGuest.Document),
                    ContentType = InPersonGuest.Document.ContentType,
                    Name = InPersonGuest.Name,
                    Content = memoryStream.ToArray(),
                    RsvpId = UserId
                };

                Context.InPersonGuests.Add(guest);
                await Context.SaveChangesAsync();
            }
            return RedirectToPage("./Dashboard");
        }
    }
}
