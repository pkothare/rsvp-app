using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Opifex.Rsvp
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration Configuration;

        private readonly ISendGridClient SendGridClient;

        public EmailSender(IConfiguration configuration, ISendGridClient sendGridClient)
        {
            Configuration = configuration;
            SendGridClient = sendGridClient;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new SendGridMessage
            {
                From = new EmailAddress(Configuration.GetValue("SendGrid:From", "info@durwa-pranav.com")),
                Subject = subject,
                HtmlContent = htmlMessage
            };
            message.AddTo(email);
            await SendGridClient.SendEmailAsync(message).ConfigureAwait(false);
        }
    }
}
