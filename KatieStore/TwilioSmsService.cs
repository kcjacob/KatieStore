using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Twilio.Clients;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;


namespace KatieStore
{
    public class TwilioSmsService : IIdentityMessageService
    {
        private string _fromNumber;
        public TwilioSmsService(string accountSid, string authToken, string fromNumber)
        {
            _fromNumber = fromNumber;
            TwilioClient.Init(accountSid, authToken);
        }


        public Task SendAsync(IdentityMessage message)
        {
            CreateMessageOptions options = new CreateMessageOptions(new Twilio.Types.PhoneNumber(message.Destination));
            options.Body = message.Body;
            options.From = new Twilio.Types.PhoneNumber(_fromNumber);
            return MessageResource.CreateAsync(options);
        }
    }
}