# RSVP Application

## Overview

This is a starter application for anyone that wants to manage event RSVPs. 
This application is what I used for my own wedding, which is why you'll find a lot of references to my name in there 🙂. It's a vanilla .NET Core 3.1 app that uses Razor Pages and SendGrid integration to send emails. 

## Features

The application is meant to provide comprehensive management of RSVPs for events across a variety of roles. Normal users can RSVP to an event and add guests based on constraints specified when seeding data. Managers can view a full list of guests and their details. This role is usually applied to persons that manage the event location. This way, the manager can check IDs at the door, if needed. Finally administrators have the additional capability of sending email notifications to their guests. The information below gives further detail about each of the distinct features.

### Seeding Data
Seeding of initial set of users using [InitialData.csv](src/Opifex.Rsvp/Data/InitialData.csv) (modify the [`SeedData`](src/Opifex.Rsvp/Data/SeedData.cs) as needed). The username is bound to an email address. You can also set a display name, an RSVP constraint if you want the users to only select from a specific set of RSVP options and also constrain the maximum number of guests they are allowed to add. Lastly, you can also assign a role to them via populating the data.

### Forgot Password & Password Reset
Users that have been seeded in the system will need to setup a new password to sign in. The forgot password process, allows users to request a one time link to be sent to their email to reset their passowrd. This process works for new and existing users.

### Privileged Access
Additional access control with two distinct roles: Administrator, Manager. ASP.NET Core already provides some basic access control for anonymous and authenticated users. The two additional roles provide the ability to view the entire list of users via a dashboard
and take additional action against them.

### RSVP
Users will have the ability to RSVP for the event that is constrained based on the options that were seeded. The two default OOB options that are provided are **InPerson** and **Zoom**. See [`RsvpOptions`](src/Opifex.Rsvp/Data/RsvpOptions.cs) to discover references and how to augment it accordingly.

### Guest Management
Each user will be able to manage a list of their guests that are attending the event. The total number of guests they can add is constrained by the `MaxGuests` data that was seeded. For users that RSVP'ed for Zoom only an email is required. For in person attendance, users have to submit identifying information, so that managers that have been given access can cross-check their IDs.

### Email Notifications
Administrators can send emails to users from the three dashboard pages: [`Admin`](src/Opifex.Rsvp/Pages/Admin.cshtml), [`InPersonGuests`](src/Opifex.Rsvp/Pages/InPersonGuests.cshtml) and [`ZoomGuests`](src/Opifex.Rsvp/Pages/ZoomGuests.cshtml). Modify and augment as needed. The template is geared to use [SendGrid](https://sendgrid.com/) to send emails. If you're depolying this to Azure it's relatively easy to setup and configure. On [startup](src/Opifex.Rsvp/Startup.cs), the application specifically looks for a SendGrid API Key.

## Development
If you want to get started with developing locally, you'll need the following:
- [SQL Server Express LocalDB](https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb?view=sql-server-ver15)
- [ASP.NET Core 5.0](https://dotnet.microsoft.com/download/dotnet/5.0)
- [Visual Studio Code](https://code.visualstudio.com/) (Optional)

Once you've downloaded and set everything up, clone (or fork and clone) this repository. From there change your working directory to `src\Opifex.Rsvp` and use the `dotnet` commands to get going. For more information see [.NET CLI overview](https://docs.microsoft.com/en-us/dotnet/core/tools/).

To run the application simple execute `dotnet run` and it will launch serve the app on https://localhost:5001 by default.

## Contributing
If you observe an issue see if it's related to any of the existing ones and join the conversation. If not, open a new issue and describe it in detail. If you want to submit a change, open an issue first and then submit a pull request associated with the issue accordingly. Last but not least please follow [GitHub Community Guidelines](https://docs.github.com/en/free-pro-team@latest/github/site-policy/github-community-guidelines).
