using System;

namespace Opifex.Rsvp.Data
{
    [Flags]
    public enum RsvpOptions : short
    {
        NotAttending = 0,
        Zoom = 1,
        InPerson = 2
    }
}
