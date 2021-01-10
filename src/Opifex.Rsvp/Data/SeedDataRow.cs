using System;

namespace Opifex.Rsvp.Data
{
    public class SeedDataRow
    {
        public string UserName { get; set; }
        public string Role { get; set; }
        public string DisplayName { get; set; }
        public RsvpOptions OptionCombinations { get; set; }
        public int MaxGuests { get; set; }
    }
}