using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KatieStore
{
    public class SmartyStreetsService : IAddressValidationService
    {
        private SmartyStreets.USStreetApi.Client _client;
        public SmartyStreetsService(string authId, string authToken)
        {
            SmartyStreets.ClientBuilder builder = new SmartyStreets.ClientBuilder(authId, authToken);

            _client = builder.BuildUsStreetApiClient();
        }

        public CheckedAddress[] CheckAddress(string street1, string street2, string city, string state, string postalCode)
        {
            CheckedAddress[] result = new CheckedAddress[0];
            //object result = null;
            if (!string.IsNullOrEmpty(street1) && !string.IsNullOrEmpty(city) && !string.IsNullOrEmpty(state)
                //&& (Request.UrlReferrer != null) && (Request.UrlReferrer.Host == Request.Url.Host)     //Uncommenting this will make sure your validate function only works on your site
                )
            {


                SmartyStreets.USStreetApi.Lookup lookup = new SmartyStreets.USStreetApi.Lookup();
                lookup.Street = street1;
                lookup.Street2 = street2;
                lookup.City = city;
                lookup.State = state;
                lookup.ZipCode = postalCode;
                _client.Send(lookup);
                result = lookup.Result.Select(x => new CheckedAddress { Street1 = x.DeliveryLine1, Street2 = x.DeliveryLine2, City = x.Components.CityName, State = x.Components.State, PostalCode = x.Components.ZipCode }).ToArray();
            }
            return result;
        }
    }

    public class CheckedAddress
    {
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
    }
}