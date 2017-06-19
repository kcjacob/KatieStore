using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Braintree;


namespace KatieStore.Models
{
    public class CheckoutModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public string ShippingFirstName { get; set; }
        public string ShippingLastName { get; set; }
        public string ShippingCompany { get; set; }
        public string ShippingAddressLine1 { get; set; }
        public string ShippingAddressLine2 { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingState { get; set; }
        public string ShippingPostalCode { get; set; }
        public string ShippingCountry { get; set; }

        public string BillingFirstName { get; set; }
        public string BillingLastName { get; set; }

        public string BillingCompany { get; set; }
        public string BillingAddressLine1 { get; set; }
        public string BillingAddressLine2 { get; set; }
        public string BillingCity { get; set; }
        public string BillingState { get; set; }
        public string BillingPostalCode { get; set; }
        public string BillingCountry { get; set; }

        public Braintree.Address[] SavedAddresses { get; set; }

        public Basket Basket { get; set; }

        public string CardToken { get; set; }
        public string BillingAddressId { get; set; }

        public string ShippingAddressId { get; set; }
        [Display(Name = "Number")]
        public string CreditCardNumber { get; set; }
        [Display(Name = "Expiration")]
        public int CreditCardExpirationMonth { get; set; }
        [Display(Name = "Expiration")]
        public int CreditCardExpirationYear { get; set; }
        [Display(Name = "CVV")]
        public string CreditCardVerificationValue { get; set; }

        public CreditCard[] SavedCards { get; set; }
        public string CustomerId { get; set; }
    }
}