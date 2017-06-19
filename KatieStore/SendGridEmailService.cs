using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Text.RegularExpressions;

namespace KatieStore
{
    public class SendGridEmailService : IIdentityMessageService
    {
        private ISendGridClient _client;

        public SendGridEmailService(string apiKey)
        {
            _client = new SendGridClient(apiKey);
        }

        public SendGridEmailService(ISendGridClient client)
        {
            _client = client;
        }

        public Task SendAsync(IdentityMessage message)
        {
            EmailAddress from = new EmailAddress("ThankYou@ogr.com", "Outdoor Gear Rental");
            EmailAddress to = new EmailAddress(message.Destination);
            SendGridMessage sendGridMessage = MailHelper.CreateSingleEmail(from, to, message.Subject, Regex.Replace(message.Body, @"<(.|\n)*?>", ""), message.Body);
            return _client.SendEmailAsync(sendGridMessage);
        }
    }
}