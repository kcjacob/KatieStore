using KatieStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using Braintree;
using Microsoft.AspNet.Identity;

namespace KatieStore.Controllers
{
    public class CheckoutController : Controller
    {
        private IAddressValidationService avs;
        private KatieStoreEntities2 db;
        private IBraintreeGateway _braintreeGateway;
        private IIdentityMessageService _emailService;
        private IIdentityMessageService _smsService;

        public CheckoutController()
        {
            _braintreeGateway = new BraintreeGateway
            {
                Environment = Braintree.Environment.SANDBOX,
                MerchantId = ConfigurationManager.AppSettings["Braintree.MerchantId"],
                PublicKey = ConfigurationManager.AppSettings["Braintree.PublicKey"],
                PrivateKey = ConfigurationManager.AppSettings["Braintree.PrivateKey"]
            };
            _emailService = new SendGridEmailService(ConfigurationManager.AppSettings["SendGrid.ApiKey"]);

            _smsService = new TwilioSmsService(
                ConfigurationManager.AppSettings["Twilio.AccountSid"],
                ConfigurationManager.AppSettings["Twilio.AuthToken"],
                ConfigurationManager.AppSettings["Twilio.FromNumber"]);


        }


    

        public async Task<ActionResult> Index()
        {
            CheckoutModel model = new CheckoutModel();
            model.SavedCards = new CreditCard[0];
            model.SavedAddresses = new Braintree.Address[0];

            if (User.Identity.IsAuthenticated)
            {
                Braintree.CustomerSearchRequest search = new Braintree.CustomerSearchRequest();
                string email = db.AspNetUsers.Single(x => x.UserName == User.Identity.Name).Email;
                search.Email.Is(email);
                var searchResult = await _braintreeGateway.Customer.SearchAsync(search);
                Braintree.Customer customer = searchResult.FirstItem;
                model.SavedCards = customer.CreditCards;
                model.SavedAddresses = customer.Addresses;
                model.CustomerId = customer.Id;
                model.FirstName = customer.FirstName;
                model.LastName = customer.LastName;
                model.Email = customer.Email;
                model.Phone = customer.Phone;
            }


            model.Basket = this.GetBasket(db);
            return View(model);
        }

        public ActionResult ValidateAddress(string street1, string street2, string city, string state, string postalCode)
        {

            object result = avs.CheckAddress(street1, street2, city, state, postalCode);

            return Json(result, JsonRequestBehavior.AllowGet);

        }



        // POST: Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(CheckoutModel model)
        {
            //Check if the model-state is valid -- this will catch anytime someone hacks your client-side validation
            if (ModelState.IsValid)
            {

                if (model.CustomerId == null)
                {
                    BraintreeService service = new BraintreeService(this._braintreeGateway);
                    model.CustomerId = await service.GetCustomerId(model.Email, model.Phone, model.FirstName, model.LastName);

                }
                if ((model.CardToken == "NewCard") || (model.CardToken == null))
                {
                    Braintree.CreditCardRequest card = new Braintree.CreditCardRequest();
                    card.Number = model.CreditCardNumber;
                    card.CVV = model.CreditCardVerificationValue;
                    card.ExpirationMonth = model.CreditCardExpirationMonth.ToString().PadLeft(2, '0');
                    card.ExpirationYear = model.CreditCardExpirationYear.ToString();
                    card.CardholderName = model.BillingFirstName + " " + model.BillingLastName;
                    card.CustomerId = model.CustomerId;
                    var cardResult = await _braintreeGateway.CreditCard.CreateAsync(card);
                    model.CardToken = cardResult.Target.Token;
                }

                if ((model.BillingAddressId == "NewAddress") || (model.BillingAddressId == null))
                {
                    Braintree.AddressRequest address = new Braintree.AddressRequest();
                    address.StreetAddress = model.BillingAddressLine1 + " " + model.BillingAddressLine2;
                    address.CountryName = model.BillingCountry;
                    address.Locality = model.BillingCity;
                    address.PostalCode = model.BillingPostalCode;
                    address.Region = model.BillingState;

                    var addressResult = await _braintreeGateway.Address.CreateAsync(model.CustomerId, address);
                    model.BillingAddressId = addressResult.Target.Id;
                }
                if ((model.ShippingAddressId == "NewAddress") || (model.ShippingAddressId == null))
                {
                    if ((model.ShippingAddressLine1 == model.BillingAddressLine1) &&
                        (model.ShippingAddressLine2 == model.BillingAddressLine2) &&
                        (model.ShippingCity == model.BillingCity) &&
                        (model.ShippingPostalCode == model.BillingPostalCode) &&
                        (model.ShippingState == model.BillingState) &&
                        (model.ShippingCountry == model.BillingCountry))
                    {
                        model.ShippingAddressId = model.BillingAddressId;
                    }
                    else
                    {
                        Braintree.AddressRequest address = new Braintree.AddressRequest();
                        address.StreetAddress = model.ShippingAddressLine1 + " " + model.ShippingAddressLine2;
                        address.CountryName = model.ShippingCountry;
                        address.Locality = model.ShippingCity;
                        address.PostalCode = model.ShippingPostalCode;
                        address.Region = model.ShippingState;

                        var addressResult = await _braintreeGateway.Address.CreateAsync(model.CustomerId, address);
                        model.ShippingAddressId = addressResult.Target.Id;
                    }
                }


                Shipment s = new Shipment
                {
                    AddressLine1 = model.ShippingAddressLine1,
                    AddressLine2 = model.ShippingAddressLine2,
                    City = model.ShippingCity,
                    State = model.ShippingState,
                    PostalCode = model.ShippingPostalCode,
                    Country = model.ShippingCountry,
                    Modified = DateTime.UtcNow,
                    Created = DateTime.UtcNow
                };


                Purchase p = new Purchase
                {
                    SubmittedDate = DateTime.UtcNow,
                    AspNetUserID = User.Identity.IsAuthenticated ? db.AspNetUsers.First(x => x.UserName == User.Identity.Name).Id : null,
                    Created = DateTime.UtcNow,
                    Modified = DateTime.UtcNow,
                    OrderIdentifier = Guid.NewGuid().ToString().Substring(0, 8),
                    PurchaseProducts = this.GetBasket(db).BasketProducts.Select(x => new Models.PurchaseProduct
                    {
                        ProductID = x.ProductID,
                        Quantity = x.Quantity,
                        Created = DateTime.UtcNow,
                        Modified = DateTime.UtcNow,
                        ProductName = x.Product.Name,
                        ProductPrice = x.Product.Price,
                        Shipment = s,
                    }).ToArray()
                };


                db.Purchases.Add(p);

                db.Baskets.Remove(this.GetBasket(db));
                db.SaveChanges();

                Braintree.TransactionRequest transaction = new Braintree.TransactionRequest();
                transaction.Amount = this.GetBasket(db).BasketProducts.Sum(x => x.Quantity * (x.Product.Price ?? 0));
                transaction.CustomerId = model.CustomerId;
                transaction.PaymentMethodToken = model.CardToken;
                transaction.OrderId = p.ID.ToString();
                transaction.PurchaseOrderNumber = p.OrderIdentifier;
                var transactionResult = await _braintreeGateway.Transaction.SaleAsync(transaction);

                await _emailService.SendAsync(new Microsoft.AspNet.Identity.IdentityMessage
                {
                    Subject = string.Format("Your Coding Cookware Order {0}", p.OrderIdentifier),
                    Destination = model.Email,
                    Body = CreateReceiptEmail(p)
                });

                if (!string.IsNullOrEmpty(model.Phone))
                {

                    await _smsService.SendAsync(new Microsoft.AspNet.Identity.IdentityMessage
                    {
                        Subject = "",
                        Destination = model.Phone,
                        Body = "You placed order " + p.OrderIdentifier
                    });
                }

                return RedirectToAction("Index", "Receipt", new { id = p.OrderIdentifier });
            };
            return View();

        }



        private static string CreateReceiptEmail(Purchase p)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<table>");
            builder.Append("<thead><tr><th></th><th>Name</th><th>Description</th><th>Unit Price</th><th>Quantity</th><th>Total Price</th></tr></thead>");
            builder.Append("<tbody>");
            foreach (var product in p.PurchaseProducts)
            {
                builder.Append("<tr><td></td>");
                builder.Append("<td>");
                builder.Append(product.ProductName);
                builder.Append("</td>");

                builder.Append("<td>");
                builder.Append(product.Product.Description);
                builder.Append("</td>");
                builder.Append("<td>");
                builder.Append((product.ProductPrice ?? 0).ToString("c"));
                builder.Append("</td>");
                builder.Append("<td>");
                builder.Append(product.Quantity);
                builder.Append("</td>");
                builder.Append("<td>");
                builder.Append(((product.ProductPrice ?? 0) * (product.Quantity)).ToString("c"));
                builder.Append("</td>");

                builder.Append("</tr>");
            }
            builder.Append("</tbody><tfoot><tr><td colspan=\"5\">Total</td><td>");
            builder.Append(p.PurchaseProducts.Sum(x => (x.Product.Price ?? 0) * x.Quantity).ToString("c"));
            builder.Append("</td></tr></tfoot></table>");
            return builder.ToString();
        }

    }
}