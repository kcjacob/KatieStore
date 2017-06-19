using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System.Configuration;

[assembly: Microsoft.Owin.OwinStartup(typeof(KatieStore.Startup))]

namespace KatieStore
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //TODO: add stuff to "app" to set up login / authentication
            // Enable the application to use a cookie to store information for the signed in user
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new Microsoft.Owin.PathString("/Account/LogOn")
            });


            app.CreatePerOwinContext(() =>
            {
                UserStore<IdentityUser> store = new UserStore<IdentityUser>();
                UserManager<IdentityUser> manager = new UserManager<IdentityUser>(store);
                manager.UserTokenProvider = new EmailTokenProvider<IdentityUser>();

                manager.PasswordValidator = new PasswordValidator
                {
                    RequiredLength = 4,
                    RequireDigit = false,
                    RequireUppercase = false,
                    RequireLowercase = false,
                    RequireNonLetterOrDigit = false

                };

                manager.EmailService = new SendGridEmailService(ConfigurationManager.AppSettings["SendGrid.ApiKey"]);
                manager.SmsService = new TwilioSmsService(
                    ConfigurationManager.AppSettings["Twilio.AccountSid"],
                    ConfigurationManager.AppSettings["Twilio.AuthToken"],
                    ConfigurationManager.AppSettings["Twilio.FromNumber"]);


                return manager;
            });

        }
    }
}