using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class CountrySoldToCustomerFilter : ICountryFilter
    {
        public int CustomerId { get; set; }

        public DateTime? EffectiveOn { get; set; }

        public bool IsEffectiveInFuture { get; set; }

        public bool IsExcluded(ICountryFilterContext context)
        {
            if (context.Country == null)
                throw new MissingArgumentValidationException("context.Country");
            CustomerCountryManager customerCountryManager = new CustomerCountryManager();
            var customerCountries = customerCountryManager.GetCustomerCountryIds(CustomerId, EffectiveOn, IsEffectiveInFuture);
            if (customerCountries == null)
                return true;

            return (!customerCountries.Contains(context.Country.CountryId));
        }
    }
}
