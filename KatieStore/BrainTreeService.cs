using Braintree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace KatieStore
{
    public class BraintreeService
    {
        IBraintreeGateway _braintreeGateway = null;

        public BraintreeService(IBraintreeGateway braintreeGateway = null)
        {
            _braintreeGateway = braintreeGateway;
        }

        public async Task<string> GetCustomerId(string email, string phone, string firstName, string lastName)
        {
            Braintree.CustomerSearchRequest search = new Braintree.CustomerSearchRequest();
            search.Email.Is(email);
            var searchResult = await _braintreeGateway.Customer.SearchAsync(search);
            if (searchResult.Ids.Any())
            {
                return searchResult.FirstItem.Id;
            }
            else
            {
                Braintree.CustomerRequest customer = new Braintree.CustomerRequest();
                customer.Email = email;
                customer.Phone = phone;
                customer.FirstName = firstName;
                customer.LastName = lastName;

                var customerResult = await _braintreeGateway.Customer.CreateAsync(customer);
                return customerResult.Target.Id;
            }
        }
    }
}