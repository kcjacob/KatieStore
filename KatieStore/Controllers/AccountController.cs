using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Braintree;
using KatieStore.Models;



namespace KatieStore.Controllers
{
    public class AccountController : Controller


    {

        public AccountController()
        {
            db = new KatieStoreEntities2();

            _braintreeGateway = new BraintreeGateway
            {
                Environment = Braintree.Environment.SANDBOX,
                MerchantId = ConfigurationManager.AppSettings["Braintree.MerchantId"],
                PublicKey = ConfigurationManager.AppSettings["Braintree.PublicKey"],
                PrivateKey = ConfigurationManager.AppSettings["Braintree.PrivateKey"]
            };
        }
        

        public AccountController(KatieStoreEntities2 entities = null, ControllerContext context = null, IBraintreeGateway braintreeGateway = null)
        {
            if (entities == null)
            {
                db = new KatieStoreEntities2();
            }
            else
            {
                db = entities;
            }

            if (context != null)
            {
                ControllerContext = context;
            }

            if (braintreeGateway == null)
            {
                _braintreeGateway = new BraintreeGateway
                {
                    Environment = Braintree.Environment.SANDBOX,
                    MerchantId = ConfigurationManager.AppSettings["Braintree.MerchantId"],
                    PublicKey = ConfigurationManager.AppSettings["Braintree.PublicKey"],
                    PrivateKey = ConfigurationManager.AppSettings["Braintree.PrivateKey"]
                }; ;
            }
            else
            {
                _braintreeGateway = braintreeGateway;
            }

            _currentCustomer = null;

        }

        protected KatieStoreEntities2 db;

        protected IBraintreeGateway _braintreeGateway;

        private Customer _currentCustomer;
        protected async Task<Customer> GetCurrentCustomer()
        {
            if (_currentCustomer == null)
            {
                CustomerSearchRequest search = new CustomerSearchRequest();
                string email = db.AspNetUsers.Single(x => x.UserName == User.Identity.Name).Email;
                search.Email.Is(email);
                var searchResult = await _braintreeGateway.Customer.SearchAsync(search);

                if (!searchResult.Ids.Any())
                {
                    CustomerRequest newCustomer = new CustomerRequest();
                    newCustomer.Email = email;
                    Result<Customer> createdResult = await _braintreeGateway.Customer.CreateAsync(newCustomer);
                    _currentCustomer = createdResult.Target;
                }
                else
                {
                    _currentCustomer = searchResult.FirstItem;
                }
            }
            return _currentCustomer;

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [Authorize]
        public async Task<ActionResult> Index()
        {
            ViewBag.Customer = await GetCurrentCustomer();
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Index(string id, string firstName, string lastName, string phoneNumber)
        {
            CustomerRequest customer = new CustomerRequest();
            customer.FirstName = firstName;
            customer.LastName = lastName;
            customer.Phone = phoneNumber;
            await _braintreeGateway.Customer.UpdateAsync(id, customer);
            return RedirectToAction("index");
        }

        [Authorize]
        public async Task<ActionResult> CreditCard()
        {
            Customer customer = await GetCurrentCustomer();
            return View(customer.CreditCards);

        }

        [Authorize]
        public async Task<ActionResult> CreditCardDelete(string id)
        {
            Customer customer = await GetCurrentCustomer();
            var card = customer.CreditCards.FirstOrDefault(x => x.UniqueNumberIdentifier == id);
            await _braintreeGateway.CreditCard.DeleteAsync(card.Token);
            return RedirectToAction("CreditCard");
        }

        // GET: Account/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(string username, string email, string password, string phone, string firstName, string lastName)
        {
            UserManager<IdentityUser> manager = HttpContext.GetOwinContext().Get<UserManager<IdentityUser>>();

            if (ModelState.IsValid)
            {
                var user = new IdentityUser()
                {
                    UserName = username,
                    Email = email,
                    EmailConfirmed = false,
                };

                var result = await manager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    BraintreeService service = new BraintreeService(_braintreeGateway);
                    await service.GetCustomerId(email, phone, firstName, lastName);

                    string confirmationToken = await manager.GenerateEmailConfirmationTokenAsync(user.Id);
                    string confirmationLink = Request.Url.GetLeftPart(UriPartial.Authority) + "/Account/Confirm/" + user.Id + "?token=" + confirmationToken;
                    string htmlContent = string.Format("<a href=\"{0}\">Confirm Your Account</a>", confirmationLink);
                    await manager.SendEmailAsync(user.Id, "Confirm your Outdoor Gear Rental Account", htmlContent);

                    TempData["EmailAddress"] = email;
                    return RedirectToAction("ConfirmationSent");
                }
                else
                {
                    ViewBag.Error = result.Errors;
                }
            }
            return View();
        }

        public ActionResult ConfirmationSent()
        {
            return View();
        }

        // GET Account/LogOff
        public ActionResult LogOff()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        // GET Account/LogOn
        public ActionResult LogOn()
        {
            return View();
        }

        // POST Account/LogOn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LogOn(string username, string password, bool? staySignedIn, string returnUrl)
        {
            UserManager<IdentityUser> manager = HttpContext.GetOwinContext().Get<UserManager<IdentityUser>>();
            var user = await manager.FindByNameAsync(username);

            bool result = await manager.CheckPasswordAsync(user, password);
            if (result)
            {
                if (!user.EmailConfirmed)
                {
                    ViewBag.Error = new string[] { "Your email address has not been confirmed." };
                }
                else if (user.LockoutEnabled)
                {
                    ViewBag.Error = new string[] { string.Format("Your account is locked out until {0}", user.LockoutEndDateUtc.Value.ToLocalTime().ToString()) };
                }
                else
                {
                    var userIdentity = await manager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                    AuthenticationProperties authenticationProperties = new AuthenticationProperties
                    {
                        IsPersistent = staySignedIn ?? false
                    };
                    HttpContext.GetOwinContext().Authentication.SignIn(authenticationProperties, userIdentity);
                    if (string.IsNullOrEmpty(returnUrl))
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return Redirect(returnUrl);
                    }
                }
            }
            else
            {
                ViewBag.Error = new string[] { "Unable to Log In, check your username and password" };
            }
            return View();
        }

        public async Task<ActionResult> Confirm(string id, string token)
        {
            UserManager<IdentityUser> manager = HttpContext.GetOwinContext().Get<UserManager<IdentityUser>>();
            var user = await manager.FindByIdAsync(id);
            if (user != null)
            {
                var result = await manager.ConfirmEmailAsync(user.Id, token);
                if (result.Succeeded)
                {
                    TempData["Confirmed"] = true;
                    return RedirectToAction("LogOn");
                }
            }
            return HttpNotFound();
        }

        /// <summary>
        /// Display a form for a user to enter their username / email address
        /// </summary>
        /// <returns></returns>
        public ActionResult ForgotPassword()
        {
            return View();
        }

        /// <summary>
        /// Validate the posted information and send an email with a reset token
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(string email, string username)
        {
            UserManager<IdentityUser> manager = HttpContext.GetOwinContext().Get<UserManager<IdentityUser>>();
            IdentityUser user = null;
            if (!string.IsNullOrEmpty(email))
            {
                user = await manager.FindByEmailAsync(email);
            }
            else if (!string.IsNullOrEmpty(username))
            {
                user = await manager.FindByNameAsync(username);
            }

            if (user != null)
            {
                string resetToken = await manager.GeneratePasswordResetTokenAsync(user.Id);
                string resetLink = Request.Url.GetLeftPart(UriPartial.Authority) + "/Account/ResetPassword/" + user.Id + "?token=" + resetToken;

                string htmlContent = string.Format("<a href=\"{0}\">Reset Your Password</a>", resetLink);
                await manager.SendEmailAsync(user.Id, "Reset your Outdoor Gear Rental Password", htmlContent);
            }

            //Even if the user was not found, we'll still redirect to the reset password sent page.
            return RedirectToAction("ResetPasswordSent");
        }


        /// <summary>
        /// Simple - Return a view
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetPasswordSent()
        {
            return View();
        }

        //validate the reset token and display a form if it is valid
        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(string id, string token, string password)
        {
            if (ModelState.IsValid)
            {
                UserManager<IdentityUser> manager = HttpContext.GetOwinContext().Get<UserManager<IdentityUser>>();
                var user = await manager.FindByIdAsync(id);
                if (user != null)
                {

                    var result = await manager.ResetPasswordAsync(id, token, password);
                    if (result.Succeeded)
                    {
                        user.EmailConfirmed = true;
                        manager.Update(user);
                        TempData["Reset"] = true;
                        return RedirectToAction("LogOn");
                    }
                    else
                    {
                        ViewBag.Error = result.Errors;
                    }
                }
            }
            return View();
        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(string currentPassword, string newPassword)
        {
            if (ModelState.IsValid)
            {
                UserManager<IdentityUser> manager = HttpContext.GetOwinContext().Get<UserManager<IdentityUser>>();
                var user = await manager.FindByNameAsync(User.Identity.Name);

                var result = await manager.ChangePasswordAsync(user.Id, currentPassword, newPassword);
                if (result.Succeeded)
                {
                    TempData["PasswordChanged"] = true;
                    return RedirectToAction("Index");
                }
                ViewBag.Errors = result.Errors;
            }
            return View();
        }
    }
}